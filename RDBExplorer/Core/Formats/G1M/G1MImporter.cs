using Metanoia.Modeling;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RDBExplorer.Core.Formats.G1M
{
    internal class G1MImporter
    {
        private G1MData _data = new G1MData();
        private Vector3[] _boneWorldPositions;
        private Quaternion[] _boneWorldRotations;
        private List<Vector3[]> _nunoWorldPoints = new List<Vector3[]>();

        private void Log(string msg)
        {
#if DEBUG
            Console.WriteLine(msg);
#endif
        }

        public void Open(string path)
        {
            Log($"[Open] file = {path}");

            byte[] data = File.ReadAllBytes(path);
            Open(data);
        }

        public void Open(byte[] data)
        {

            using (var reader = new BinaryReader(new MemoryStream(data)))
                _data.Parse(reader);

            Log($"[Open] Skeleton bones   : {_data.Skeleton.Count}");
            Log($"[Open] VertexBuffers    : {_data.VertexBuffers.Count}");
            Log($"[Open] IndexBuffers     : {_data.IndexBuffers.Count}");
            Log($"[Open] Layouts          : {_data.Layouts.Count}");
            Log($"[Open] BonePalettes     : {_data.BonePalettes.Count}");
            Log($"[Open] Submeshes        : {_data.Submeshes.Count}");

            for (int i = 0; i < _data.VertexBuffers.Count; i++)
                Log($"  VB[{i}] stride={_data.VertexBuffers[i].Stride}  bytes={_data.VertexBuffers[i].Data.Length}  " +
                    $"verts~{(_data.VertexBuffers[i].Stride > 0 ? _data.VertexBuffers[i].Data.Length / _data.VertexBuffers[i].Stride : 0)}");

            for (int i = 0; i < _data.IndexBuffers.Count; i++)
                Log($"  IB[{i}] step={_data.IndexBuffers[i].Step}  bytes={_data.IndexBuffers[i].Data.Length}  " +
                    $"indices={_data.IndexBuffers[i].Data.Length / Math.Max(_data.IndexBuffers[i].Step, 1)}");

            for (int i = 0; i < _data.Layouts.Count; i++)
            {
                Log($"  Layout[{i}] bufRefs=[{string.Join(",", _data.Layouts[i].BufferIndices)}]  semantics={_data.Layouts[i].Semantics.Count}");
                foreach (var s in _data.Layouts[i].Semantics)
                    Log($"    sem type={s.Type} layer={s.Layer} fmt={s.Format} bufIdx={s.BufIdx} offset={s.Offset}");
            }

            for (int i = 0; i < _data.Submeshes.Count; i++)
            {
                var sm = _data.Submeshes[i];
                Log($"  SM[{i}] VBRef={sm.VBRef} IBRef={sm.IBRef} BoneMap={sm.BoneMapIndex} " +
                    $"VBStart={sm.VBStart} VertCount={sm.VertexCount} IBStart={sm.IBStart} IdxCount={sm.IndexCount} Prim={sm.PrimType}");
            }
        }

        public GenericModel ToGenericModel()
        {
            var model = new GenericModel { Name = "G1M_Model" };

            if (_data.Skeleton.Count > 0)
            {
                model.Skeleton = new GenericSkeleton();
                foreach (var b in _data.Skeleton)
                {
                    model.Skeleton.Bones.Add(new GenericBone
                    {
                        Name = b.Name,
                        ParentIndex = b.ParentIndex,
                        Position = b.Position,
                        Rotation = b.Rotation,
                        Scale = b.Scale
                    });
                }
            }

            ComputeBoneWorldTransforms();
            PrecomputeNunoBones();
            int meshesAdded = 0;

            var submeshClothId = new Dictionary<int, int>();
            var submeshExternalId = new Dictionary<int, uint>();

            foreach (var matInternal in _data.Materials)
            {
                if (matInternal.Textures.Count > 0)
                {
                    foreach (var tex in matInternal.Textures)
                    {
                        var genMat = new GenericMaterial();
                        genMat.TextureDiffuse = $"Texture_{tex.Index}";
                        genMat.EnableBlend = true;
                        model.MaterialBank[$"Material_{tex.Index}"] = genMat;
                    }
                }
            }

            foreach (var group in _data.MeshGroups)
            {
                foreach (var m in group.Meshes)
                {
                    foreach (var subIdx in m.SubmeshIndices)
                    {
                        submeshClothId[(int)subIdx] = m.ClothID;
                        submeshExternalId[(int)subIdx] = m.ExternalID;
                    }
                }

            }

            foreach (var sm in _data.Submeshes)
            {
                var ib = _data.IndexBuffers[sm.IBRef];
                var layout = _data.Layouts[sm.VBRef];
                var palette = (sm.BoneMapIndex < _data.BonePalettes.Count) ? _data.BonePalettes[sm.BoneMapIndex] : null;

                var rawIndices = ib.GetIndices(sm.IBStart, sm.IndexCount);
                var mesh = new GenericMesh
                {
                    Name = $"Submesh_{sm.ID}",
                    MaterialName = $"Material_{sm.MaterialIndex}",
                    PrimitiveType = PrimitiveType.Triangles
                };

                uint restartIndex = (ib.Step == 4) ? 0xFFFFFFFF : 0xFFFF;
                if (sm.PrimType == 4)
                {
                    int winding = 0;
                    for (int i = 0; i < rawIndices.Length - 2; i++)
                    {
                        uint idx1 = rawIndices[i], idx2 = rawIndices[i + 1], idx3 = rawIndices[i + 2];
                        if (idx1 == restartIndex || idx2 == restartIndex || idx3 == restartIndex)
                        {
                            winding = 0;
                            continue;
                        }
                        if (idx1 != idx2 && idx2 != idx3 && idx1 != idx3)
                        {
                            uint r1 = (idx1 >= sm.VBStart) ? idx1 - sm.VBStart : idx1;
                            uint r2 = (idx2 >= sm.VBStart) ? idx2 - sm.VBStart : idx2;
                            uint r3 = (idx3 >= sm.VBStart) ? idx3 - sm.VBStart : idx3;
                            if (winding % 2 == 0)
                            {
                                mesh.Triangles.Add(r1);
                                mesh.Triangles.Add(r2);
                                mesh.Triangles.Add(r3);
                            }
                            else
                            {
                                mesh.Triangles.Add(r1);
                                mesh.Triangles.Add(r3);
                                mesh.Triangles.Add(r2);
                            }
                        }
                        winding++;
                    }
                }
                else
                {
                    for (int i = 0; i < rawIndices.Length; i += 3)
                    {
                        if (i + 2 >= rawIndices.Length)
                            break;
                        uint idx1 = rawIndices[i], idx2 = rawIndices[i + 1], idx3 = rawIndices[i + 2];
                        mesh.Triangles.Add((idx1 >= sm.VBStart) ? idx1 - sm.VBStart : idx1);
                        mesh.Triangles.Add((idx2 >= sm.VBStart) ? idx2 - sm.VBStart : idx2);
                        mesh.Triangles.Add((idx3 >= sm.VBStart) ? idx3 - sm.VBStart : idx3);
                    }
                }

                int clothId = submeshClothId.TryGetValue(sm.ID, out int cid) ? cid : 0;
                Vector3[] cachedNunoWorldPts = null;
                if (clothId == 1 && submeshExternalId.TryGetValue(sm.ID, out uint extId))
                {
                    int realNunoIndex = -1;
                    if (extId >= 0 && extId < 10000)
                    {
                        realNunoIndex = (int)extId;
                    }
                    else if (extId >= 10000 && extId < 20000)
                    {
                        realNunoIndex = (int)(extId % 10000);
                    }
                    else if (extId >= 20000 && extId < 30000)
                    {
                        realNunoIndex = (int)(extId % 20000);
                    }

                    if (realNunoIndex != -1 && realNunoIndex < _data.NunoEntries.Count)
                    {
                        cachedNunoWorldPts = GetNunoWorldPoints((uint)realNunoIndex);
                        // Log($"[DEBUG] Submesh {sm.ID} matched NunoEntry {realNunoIndex} via ExternalID {extId}");
                    }
                    else
                    {
                        Log($"[WARN] Submesh {sm.ID} (Cloth 1) could not find NunoEntry for ExtID {extId}. NunoEntries Count: {_data.NunoEntries.Count}");
                    }
                }

                for (int i = 0; i < (int)sm.VertexCount; i++)
                {
                    int vGlobalIdx = (int)sm.VBStart + i;
                    var v = new GenericVertex { Clr = Vector4.One, Weights = new Vector4(1, 0, 0, 0) };

                    var vSem = new Dictionary<string, Vector4>();

                    foreach (var sem in layout.Semantics)
                    {
                        var vb = _data.VertexBuffers[(int)layout.BufferIndices[sem.BufIdx]];
                        int offset = vGlobalIdx * vb.Stride + sem.Offset;
                        Vector4 val = vb.ReadVec4(offset, sem.Format);
                        vSem[$"{sem.Type}_{sem.Layer}"] = val;

                        switch (sem.Type)
                        {
                            case G1MSemanticType.POSITION:
                                v.Pos = val.Xyz;
                                break;
                            case G1MSemanticType.NORMAL:
                                v.Nrm = val.Xyz;
                                break;
                            case G1MSemanticType.TEXCOORD:
                                if (sem.Layer == 0)
                                    v.UV0 = val.Xy;
                                break;
                            case G1MSemanticType.BLENDWEIGHT:
                                v.Weights = val;
                                break;
                            case G1MSemanticType.BLENDINDICES:
                                v.Bones = Remap(val, palette);
                                break;
                            case G1MSemanticType.TANGENT:
                                v.Tan = val;
                                break;
                            case G1MSemanticType.BINORMAL:
                                v.Bit = val;
                                break;
                            case G1MSemanticType.COLOR:
                                if (sem.Layer == 0)
                                    v.Clr = val;
                                break;
                        }
                    }

                    // nuno cloths
                    if (clothId == 1 && cachedNunoWorldPts != null)
                    {
                        if (v.Bit.LengthSquared > 0.000001f)
                        {
                            uint nunoIdx = submeshExternalId[sm.ID] % 10000;
                            var worldPts = _nunoWorldPoints[(int)nunoIdx];

                            vSem.TryGetValue("BLENDINDICES_0", out var idx1);
                            vSem.TryGetValue("PSIZE_0", out var idx2);
                            vSem.TryGetValue("FOG_0", out var idx3);
                            vSem.TryGetValue("TEXCOORD_5", out var idx4);

                            Vector4 cpWeights1 = vSem["POSITION_0"];
                            Vector4 cpWeights2 = vSem["BINORMAL_0"];
                            Vector4 comWeights1 = vSem["BLENDWEIGHT_0"];
                            vSem.TryGetValue("COLOR_1", out var comWeights2);

                            Vector3 GetPoint(Vector4 indices, Vector4 weights)
                            {
                                Vector3 res = Vector3.Zero;

                                if (weights.X != 0 && (int)indices.X < worldPts.Length)
                                    res += worldPts[(int)indices.X] * weights.X;
                                if (weights.Y != 0 && (int)indices.Y < worldPts.Length)
                                    res += worldPts[(int)indices.Y] * weights.Y;
                                if (weights.Z != 0 && (int)indices.Z < worldPts.Length)
                                    res += worldPts[(int)indices.Z] * weights.Z;
                                if (weights.W != 0 && (int)indices.W < worldPts.Length)
                                    res += worldPts[(int)indices.W] * weights.W;
                                return res;
                            }

                            Vector3 u1 = GetPoint(idx1, cpWeights1); Vector3 v1 = GetPoint(idx1, cpWeights2);
                            Vector3 u2 = GetPoint(idx2, cpWeights1); Vector3 v2 = GetPoint(idx2, cpWeights2);
                            Vector3 u3 = GetPoint(idx3, cpWeights1); Vector3 v3 = GetPoint(idx3, cpWeights2);
                            Vector3 u4 = GetPoint(idx4, cpWeights1); Vector3 v4 = GetPoint(idx4, cpWeights2);

                            Vector3 a = u1 * comWeights1.X + u2 * comWeights1.Y + u3 * comWeights1.Z + u4 * comWeights1.W;
                            Vector3 b = Vector3.Zero;
                            Vector3 c = Vector3.Zero;

                            if (comWeights2.LengthSquared > 0)
                            {
                                b = u1 * comWeights2.X + u2 * comWeights2.Y + u3 * comWeights2.Z + u4 * comWeights2.W;
                            }
                            else
                            {
                                b = u1 * comWeights1.X + u2 * comWeights1.Y + u3 * comWeights1.Z + u4 * comWeights1.W;
                            }
                            c = v1 * comWeights1.X + v2 * comWeights1.Y + v3 * comWeights1.Z + v4 * comWeights1.W;

                            if (b.LengthSquared > 0)
                                b.Normalize();
                            if (c.LengthSquared > 0)
                                c.Normalize();

                            Vector3 d = Vector3.Cross(b, c);
                            if (d.LengthSquared > 0.000001f)
                                d.Normalize();

                            vSem.TryGetValue("NORMAL_0", out var nrmVal);
                            float depth = nrmVal.W;

                            v.Pos = a + (d * depth);
                            Vector3 localNormal = nrmVal.Xyz;

                            v.Nrm = (b * localNormal.Y + c * localNormal.X + d * localNormal.Z).Normalized();

                            if (v.Tan.LengthSquared > 0)
                            {
                                Vector3 localTan = v.Tan.Xyz;
                                Vector3 worldTan = (b * localTan.Y + c * localTan.X + d * localTan.Z).Normalized();
                                v.Tan = new Vector4(worldTan, v.Tan.W);
                            }
                        }
                        else
                        {
                            if (palette != null && palette.Count > 0)
                            {
                                uint gIdx = palette[0];
                                if (gIdx < _boneWorldPositions.Length)
                                {
                                    v.Pos = Vector3.Transform(v.Pos, _boneWorldRotations[gIdx]) + _boneWorldPositions[gIdx];
                                }
                            }
                        }
                    }
                    else if (clothId == 2)
                    {
                        if (sm.BoneMapIndex >= 0 && sm.BoneMapIndex < _data.PhysicsPalettes.Count)
                        {
                            var physPal = _data.PhysicsPalettes[sm.BoneMapIndex];

                            vSem.TryGetValue("BLENDINDICES_0", out var bi0);
                            int localIdx = (int)(Math.Round(bi0.X) / 3.0);

                            if (localIdx >= 0 && localIdx < physPal.Count)
                            {
                                uint globalBoneIdx = physPal[localIdx];

                                if (globalBoneIdx < _boneWorldPositions.Length)
                                {
                                    Vector3 bonePos = _boneWorldPositions[globalBoneIdx];
                                    Quaternion boneRot = _boneWorldRotations[globalBoneIdx];

                                    Vector3 rotatedPos = Vector3.Transform(v.Pos, boneRot);
                                    v.Pos = bonePos + rotatedPos;
                                    v.Nrm = Vector3.Transform(v.Nrm, boneRot).Normalized();
                                }
                            }
                        }
                    }

                    mesh.Vertices.Add(v);
                }

                if (mesh.Vertices.Count > 0 && mesh.Triangles.Count > 0)
                {
                    model.Meshes.Add(mesh);
                    meshesAdded++;
                }
            }
            return model;
        }

        private void PrecomputeNunoBones()
        {
            _nunoWorldPoints.Clear();
            if (_data.Skeleton.Count == 0 || _data.BoneIDList == null)
                return;

            foreach (var entry in _data.NunoEntries)
            {
                var worldPoints = new Vector3[entry.ControlPoints.Count];

                int parentBoneIdx = 0;
                if (entry.ParentID < _data.BoneIDList.Length)
                {
                    parentBoneIdx = _data.BoneIDList[entry.ParentID];
                    if (parentBoneIdx == 0xFFFF) parentBoneIdx = 0;
                }

                Vector3 pPos = _boneWorldPositions[parentBoneIdx];
                Quaternion pRot = _boneWorldRotations[parentBoneIdx];

                for (int i = 0; i < entry.ControlPoints.Count; i++)
                {
                    worldPoints[i] = pPos + Vector3.Transform(entry.ControlPoints[i].Xyz, pRot);
                }
                _nunoWorldPoints.Add(worldPoints);
            }
        }

        private void ComputeBoneWorldTransforms()
        {
            if (_data.Skeleton == null || _data.Skeleton.Count == 0)
                return;

            _boneWorldPositions = new Vector3[_data.Skeleton.Count];
            _boneWorldRotations = new Quaternion[_data.Skeleton.Count];

            for (int i = 0; i < _data.Skeleton.Count; i++)
            {
                var b = _data.Skeleton[i];

                if (b.ParentIndex >= 0 && b.ParentIndex < i)
                {
                    Vector3 rotatedPos = Vector3.Transform(b.Position, _boneWorldRotations[b.ParentIndex]);
                    _boneWorldPositions[i] = _boneWorldPositions[b.ParentIndex] + rotatedPos;

                    _boneWorldRotations[i] = _boneWorldRotations[b.ParentIndex] * b.Rotation;
                    _boneWorldRotations[i].Normalize();
                }
                else
                {
                    _boneWorldPositions[i] = b.Position;
                    _boneWorldRotations[i] = b.Rotation;
                }
            }
        }
        private Vector4 Remap(Vector4 raw, List<uint> palette)
        {
            if (palette == null)
            {
                return new Vector4(
                    (int)(raw.X / 3),
                    (int)(raw.Y / 3),
                    (int)(raw.Z / 3),
                    (int)(raw.W / 3)
                );
            }
            return new Vector4(
                GetPal(raw.X, palette),
                GetPal(raw.Y, palette),
                GetPal(raw.Z, palette),
                GetPal(raw.W, palette)
            );
        }

        private float GetPal(float val, List<uint> p)
        {
            int idx = (int)val / 3;
            return (idx >= 0 && idx < p.Count) ? p[idx] : 0;
        }

        private Vector3[] GetNunoWorldPoints(uint nunoIndex)
        {
            var entry = _data.NunoEntries[(int)nunoIndex];
            Vector3[] worldPts = new Vector3[entry.ControlPoints.Count];

            uint globalParentIdx = entry.ParentID;

            if (globalParentIdx >= _boneWorldPositions.Length)
                globalParentIdx = 0;

            Vector3 pPos = _boneWorldPositions[globalParentIdx];
            Quaternion pRot = _boneWorldRotations[globalParentIdx];

            for (int i = 0; i < entry.ControlPoints.Count; i++)
            {
                worldPts[i] = pPos + Vector3.Transform(entry.ControlPoints[i].Xyz, pRot);
            }
            return worldPts;
        }
    }
}