using OpenTK.Graphics.OpenGL;
using Metanoia.Modeling;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using OpenTK.Mathematics;

namespace Metanoia.Rendering
{
    public enum RenderMode
    {
        Shaded,
        Textured,
        Normals,
        Colors,
        UV0,
        BoneWeight,
        Points
    }

    public class GenericRenderer
    {
        public bool HasModelSet => Model != null;
        public RenderMode RenderMode { get; set; }

        private Shader GenericShader = null;
        private Buffer VertexBuffer = null;
        private Buffer IndexBuffer = null;
        private int _vao = 0;
        private int _ssbo = 0;

        private const int MaxBones = 2048;
        private const int BufferSize = MaxBones * 64;

        private GenericSkeleton Skeleton = null;
        private GenericModel Model = null;

        private Dictionary<string, RenderTexture> Textures = new();
        private Dictionary<GenericMesh, int> MeshToOffset = new();

        public void SetGenericModel(GenericModel model)
        {
            this.Model = model;
            if (Model == null) return;

            if (GenericShader == null)
            {
                _vao = GL.GenVertexArray();

                GenericShader = new Shader();
                GenericShader.LoadShader("Rendering/Shaders/Generic.vert", ShaderType.VertexShader);
                GenericShader.LoadShader("Rendering/Shaders/Generic.frag", ShaderType.FragmentShader);
                GenericShader.CompileProgram();

                Debug.WriteLine("[Shader] Log:");
                Debug.WriteLine(GenericShader.GetErrorLog());

                VertexBuffer = new Buffer(BufferTarget.ArrayBuffer);
                IndexBuffer = new Buffer(BufferTarget.ElementArrayBuffer);

                _ssbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _ssbo);
                GL.BufferData(BufferTarget.ShaderStorageBuffer, BufferSize, System.IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _ssbo);

                int blockIdx = GL.GetUniformBlockIndex(GenericShader.ProgramID, "BoneBlock");
                if (blockIdx >= 0)
                {
                    GL.UniformBlockBinding(GenericShader.ProgramID, blockIdx, 0);
                }
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _ssbo);

                Debug.WriteLine($"[UBO] BoneBlock index={blockIdx} size={BufferSize} bytes ({MaxBones} bones)");
            }

            ClearTextures();
            Skeleton = Model.Skeleton;
            LoadBufferData(Model);

            foreach (var tex in Model.TextureBank)
            {
                var rt = new RenderTexture();
                rt.LoadGenericTexture(tex.Value);
                Textures[tex.Key] = rt;
            }

            Debug.WriteLine($"[Renderer] {Textures.Count} textures, skeleton={Skeleton?.Bones.Count ?? 0} bones");
        }

        private void LoadBufferData(GenericModel model)
        {
            MeshToOffset.Clear();
            var vertices = new List<GenericVertex>();
            var indices = new List<int>();
            int offset = 0;

            foreach (var mesh in model.Meshes)
            {
                MeshToOffset[mesh] = indices.Count;
                vertices.AddRange(mesh.Vertices);
                foreach (uint idx in mesh.Triangles)
                {
                    indices.Add((int)(idx + offset));
                }
                offset = vertices.Count;
            }

            Debug.WriteLine($"[Renderer] Upload {vertices.Count} verts, {indices.Count} indices");

            GL.BindVertexArray(_vao);
            VertexBuffer.Bind();
            GL.BufferData(VertexBuffer.BufferTarget,
                vertices.Count * GenericVertex.Stride,
                vertices.ToArray(), BufferUsageHint.StaticDraw);
            IndexBuffer.Bind();
            GL.BufferData(IndexBuffer.BufferTarget,
                indices.Count * 4,
                indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
        }

        public void ClearTextures()
        {
            foreach (var rt in Textures.Values)
            {
                rt.Delete();
            }
            Textures.Clear();
        }

        public void RenderShader(Matrix4 mvp, bool renderSkeleton = false)
        {
            if (Model == null)
            {
                return;
            }

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.UseProgram(GenericShader.ProgramID);

            // MVP
            GL.UniformMatrix4(GenericShader.GetAttributeLocation("mvp"), false, ref mvp);
            GL.Uniform1(GenericShader.GetAttributeLocation("renderMode"), (int)RenderMode);

            UploadBoneMatrices();

            int selectedBone = -1;
            if (RenderMode == RenderMode.BoneWeight && Skeleton != null)
            {
                selectedBone = Skeleton.Bones.FindIndex(b => b.Selected);
            }
            GL.Uniform1(GenericShader.GetAttributeLocation("selectedBone"), selectedBone);

            // VAO + attribs
            GL.BindVertexArray(_vao);
            VertexBuffer.Bind();
            IndexBuffer.Bind();

            int posLoc = GenericShader.GetAttributeLocation("pos");
            int nrmLoc = GenericShader.GetAttributeLocation("nrm");
            int uv0Loc = GenericShader.GetAttributeLocation("uv0");
            int clr0Loc = GenericShader.GetAttributeLocation("clr0");
            int boneLoc = GenericShader.GetAttributeLocation("bone");
            int weightLoc = GenericShader.GetAttributeLocation("weight");

            EnableAttrib(posLoc, 3, GenericVertex.Stride, 0);
            EnableAttrib(nrmLoc, 3, GenericVertex.Stride, 12);
            EnableAttrib(uv0Loc, 2, GenericVertex.Stride, 24);
            EnableAttrib(clr0Loc, 4, GenericVertex.Stride, 32);
            EnableAttrib(boneLoc, 4, GenericVertex.Stride, 48);
            EnableAttrib(weightLoc, 4, GenericVertex.Stride, 64);

            GL.Uniform1(GenericShader.GetAttributeLocation("dif"), 1);

            var sorted = Model.Meshes
                .OrderBy(m => {
                    var v = Vector3.TransformPosition(m.GetBounding().Xyz, mvp);
                    return -(v.Z + m.GetBounding().W);
                }).ToList();

            GL.PointSize(5f);

            foreach (var mesh in sorted)
            {
                if (!mesh.Visible) 
                    continue;

                GL.Uniform1(GenericShader.GetAttributeLocation("hasDif"), 0);
                GL.ActiveTexture(TextureUnit.Texture1);

                var material = Model.GetMaterial(mesh);
                if (material != null)
                {
                    if (material.TextureDiffuse != null
                        && Textures.TryGetValue(material.TextureDiffuse, out var tex)
                        && tex.Loaded)
                    {
                        tex.SetFromMaterial(material);
                        GL.Uniform1(GenericShader.GetAttributeLocation("hasDif"), 1);
                    }
                    if (material.EnableBlend)
                    {
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                    }
                    else
                        GL.Disable(EnableCap.Blend);
                }

                var primType = RenderMode == RenderMode.Points
                    ? PrimitiveType.Points : mesh.PrimitiveType;

                GL.DrawElements(primType,
                    mesh.Triangles.Count,
                    DrawElementsType.UnsignedInt,
                    MeshToOffset[mesh] * 4);
            }

            DisableAttrib(posLoc);
            DisableAttrib(nrmLoc);
            DisableAttrib(uv0Loc);
            DisableAttrib(clr0Loc);
            DisableAttrib(boneLoc);
            DisableAttrib(weightLoc);

            GL.BindVertexArray(0);
            GL.UseProgram(0);

            if (renderSkeleton && Skeleton != null)
            {
                GL.Disable(EnableCap.DepthTest);
                foreach (var bone in Skeleton.Bones)
                {
                    GL.Color3(bone.Selected
                        ? new float[] { 0.5f, 1f, 0.5f }
                        : new float[] { 1f, 0.5f, 0.5f });
                    GL.PointSize(bone.Selected ? 10f : 5f);
                    GL.Begin(PrimitiveType.Points);
                    GL.Vertex3(Vector3.TransformPosition(Vector3.Zero,
                        Skeleton.GetWorldTransform(bone, true)));
                    GL.End();
                }
                GL.LineWidth(1.5f);
                GL.Begin(PrimitiveType.Lines);
                foreach (var bone in Skeleton.Bones)
                {
                    if (bone.ParentIndex < 0) continue;
                    GL.Color3(0f, 0f, 1f);
                    GL.Vertex3(Vector3.TransformPosition(Vector3.Zero,
                        Skeleton.GetWorldTransform(bone, true)));
                    GL.Color3(0f, 1f, 0.5f);
                    GL.Vertex3(Vector3.TransformPosition(Vector3.Zero,
                        Skeleton.GetWorldTransform(Skeleton.Bones[bone.ParentIndex], true)));
                }
                GL.End();
            }

            GL.PopAttrib();
        }

        private void UploadBoneMatrices()
        {
            var boneArray = new Matrix4[MaxBones];
            for (int i = 0; i < MaxBones; i++)
            {
                boneArray[i] = Matrix4.Identity;
            }

            if (Skeleton != null)
            {
                var transforms = Skeleton.GetBindTransforms();
                int count = System.Math.Min(transforms.Length, MaxBones);
                for (int i = 0; i < count; i++)
                {
                    boneArray[i] = transforms[i];
                }
            }

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _ssbo);
            GL.BufferSubData(BufferTarget.ShaderStorageBuffer, System.IntPtr.Zero, BufferSize, boneArray);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        private static void EnableAttrib(int loc, int size, int stride, int offset)
        {
            if (loc < 0)
                return;
            GL.EnableVertexAttribArray(loc);
            GL.VertexAttribPointer(loc, size, VertexAttribPointerType.Float, false, stride, offset);
        }

        private static void DisableAttrib(int loc)
        {
            if (loc < 0) 
                return;
            GL.DisableVertexAttribArray(loc);
        }
    }
}