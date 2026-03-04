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

        private GenericSkeleton Skeleton = null;
        private GenericModel Model = null;

        private Dictionary<string, RenderTexture> Textures = new Dictionary<string, RenderTexture>();
        private Dictionary<GenericMesh, int> MeshToOffset = new Dictionary<GenericMesh, int>();

        // ─────────────────────────────────────────────────────────────
        public void SetGenericModel(GenericModel model)
        {
            // ✅ Присвоюємо ПЕРЕД перевіркою
            this.Model = model;

            if (Model == null) return;

            // Ініціалізуємо GL-ресурси один раз
            if (GenericShader == null)
            {
                // VAO — обов'язковий для 3.3+
                _vao = GL.GenVertexArray();

                GenericShader = new Shader();
                GenericShader.LoadShader("Rendering/Shaders/Generic.vert", ShaderType.VertexShader);
                GenericShader.LoadShader("Rendering/Shaders/Generic.frag", ShaderType.FragmentShader);
                GenericShader.CompileProgram();

                Debug.WriteLine("[Shader] Error Log:");
                Debug.WriteLine(GenericShader.GetErrorLog());

                VertexBuffer = new Buffer(BufferTarget.ArrayBuffer);
                IndexBuffer = new Buffer(BufferTarget.ElementArrayBuffer);
            }

            ClearTextures();

            Skeleton = Model.Skeleton;
            LoadBufferData(Model);

            // Завантажуємо текстури
            foreach (var tex in Model.TextureBank)
            {
                var rt = new RenderTexture();
                rt.LoadGenericTexture(tex.Value);
                Textures[tex.Key] = rt;
            }

            Debug.WriteLine($"[Renderer] Loaded {Textures.Count} textures");
        }

        // ─────────────────────────────────────────────────────────────
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
                    indices.Add((int)(idx + offset));
                offset = vertices.Count;
            }

            Debug.WriteLine($"[Renderer] Uploading {vertices.Count} verts, {indices.Count} indices");

            // Прив'язуємо VAO перед налаштуванням буферів
            GL.BindVertexArray(_vao);

            VertexBuffer.Bind();
            GL.BufferData(VertexBuffer.BufferTarget,
                vertices.Count * GenericVertex.Stride,
                vertices.ToArray(),
                BufferUsageHint.StaticDraw);

            IndexBuffer.Bind();
            GL.BufferData(IndexBuffer.BufferTarget,
                indices.Count * 4,
                indices.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        // ─────────────────────────────────────────────────────────────
        public void ClearTextures()
        {
            foreach (var rt in Textures.Values)
                rt.Delete();
            Textures.Clear();
        }

        // ─────────────────────────────────────────────────────────────
        public void RenderShader(Matrix4 mvp, bool renderSkeleton = false)
        {
            if (Model == null) return;

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.UseProgram(GenericShader.ProgramID);

            // ── Uniform: MVP ─────────────────────────────────────────
            int mvpLoc = GenericShader.GetAttributeLocation("mvp");
            GL.UniformMatrix4(mvpLoc, false, ref mvp);

            // ── Uniform: renderMode ──────────────────────────────────
            GL.Uniform1(GenericShader.GetAttributeLocation("renderMode"), (int)RenderMode);

            // ── Uniform: bones ───────────────────────────────────────
            int bonesLoc = GenericShader.GetAttributeLocation("bones");
            if (bonesLoc >= 0 && Skeleton != null)
            {
                var transforms = Skeleton.GetBindTransforms();
                if (transforms != null && transforms.Length > 0)
                    GL.UniformMatrix4(bonesLoc, transforms.Length, false, ref transforms[0].Row0.X);
                else
                {
                    // Fallback: одна identity матриця
                    var identity = Matrix4.Identity;
                    GL.UniformMatrix4(bonesLoc, 1, false, ref identity.Row0.X);
                }
            }

            // ── Uniform: selectedBone ────────────────────────────────
            int selectedBone = -1;
            if (RenderMode == RenderMode.BoneWeight && Skeleton != null)
                selectedBone = Skeleton.Bones.FindIndex(b => b.Selected);
            GL.Uniform1(GenericShader.GetAttributeLocation("selectedBone"), selectedBone);

            // ── Bind VAO + буфери ────────────────────────────────────
            GL.BindVertexArray(_vao);
            VertexBuffer.Bind();
            IndexBuffer.Bind();

            // ── Vertex attribs ───────────────────────────────────────
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

            // ── Сортування мешів по Z ────────────────────────────────
            var sorted = Model.Meshes
                .OrderBy(m =>
                {
                    var v = Vector3.TransformPosition(m.GetBounding().Xyz, mvp);
                    return -(v.Z + m.GetBounding().W);
                })
                .ToList();

            GL.PointSize(5f);

            // ── Draw calls ───────────────────────────────────────────
            foreach (var mesh in sorted)
            {
                if (!mesh.Visible) continue;

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
                    {
                        GL.Disable(EnableCap.Blend);
                    }
                }

                var primType = RenderMode == RenderMode.Points
                    ? PrimitiveType.Points
                    : mesh.PrimitiveType;

                GL.DrawElements(primType,
                    mesh.Triangles.Count,
                    DrawElementsType.UnsignedInt,
                    MeshToOffset[mesh] * 4);
            }

            // ── Cleanup attribs ──────────────────────────────────────
            DisableAttrib(posLoc);
            DisableAttrib(nrmLoc);
            DisableAttrib(uv0Loc);
            DisableAttrib(clr0Loc);  // ✅ виправлено з "clr" на "clr0"
            DisableAttrib(boneLoc);
            DisableAttrib(weightLoc);

            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // ── Скелет (legacy immediate mode) ──────────────────────
            if (renderSkeleton && Skeleton != null)
            {
                GL.Disable(EnableCap.DepthTest);

                foreach (var bone in Skeleton.Bones)
                {
                    GL.Color3(bone.Selected ? new float[] { 0.5f, 1f, 0.5f } : new float[] { 1f, 0.5f, 0.5f });
                    GL.PointSize(bone.Selected ? 10f : 5f);
                    GL.Begin(PrimitiveType.Points);
                    GL.Vertex3(Vector3.TransformPosition(Vector3.Zero, Skeleton.GetWorldTransform(bone, true)));
                    GL.End();
                }

                GL.LineWidth(1.5f);
                GL.Begin(PrimitiveType.Lines);
                foreach (var bone in Skeleton.Bones)
                {
                    if (bone.ParentIndex < 0) continue;
                    GL.Color3(0f, 0f, 1f);
                    GL.Vertex3(Vector3.TransformPosition(Vector3.Zero, Skeleton.GetWorldTransform(bone, true)));
                    GL.Color3(0f, 1f, 0.5f);
                    GL.Vertex3(Vector3.TransformPosition(Vector3.Zero,
                        Skeleton.GetWorldTransform(Skeleton.Bones[bone.ParentIndex], true)));
                }
                GL.End();
            }

            GL.PopAttrib();
        }

        // ─────────────────────────────────────────────────────────────
        private static void EnableAttrib(int loc, int size, int stride, int offset)
        {
            if (loc < 0) return;
            GL.EnableVertexAttribArray(loc);
            GL.VertexAttribPointer(loc, size, VertexAttribPointerType.Float, false, stride, offset);
        }

        private static void DisableAttrib(int loc)
        {
            if (loc < 0) return;
            GL.DisableVertexAttribArray(loc);
        }
    }
}