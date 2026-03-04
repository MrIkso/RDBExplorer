using Metanoia.Modeling;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Text;

namespace RDBExplorer.Core.Formats.G1M
{
    internal class G1MImporter
    {
        private G1MData _data = new G1MData();

        private void Log(string msg)
        {
            Debug.WriteLine(msg);
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
                    Log($"    sem type={s.Type}({s.RawType}) layer={s.Layer} fmt={s.Format} bufIdx={s.BufIdx} offset={s.Offset}");
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
                Log($"[Skeleton] building {_data.Skeleton.Count} bones");
                model.Skeleton = new GenericSkeleton();
                foreach (var b in _data.Skeleton)
                    model.Skeleton.Bones.Add(new GenericBone
                    {
                        Name = b.Name,
                        ParentIndex = b.ParentIndex,
                        Position = b.Position,
                        Rotation = b.Rotation,
                        Scale = b.Scale
                    });
                //model.Skeleton.RecomputeWorld();
                Log($"[Skeleton] done");
            }
            else
            {
                Log($"[Skeleton] EMPTY — no bones found");
            }

            int meshesAdded = 0;

            foreach (var sm in _data.Submeshes)
            {
                Log($"\n[Submesh {sm.ID}] VBRef={sm.VBRef} IBRef={sm.IBRef} " +
                    $"VBStart={sm.VBStart} VertCount={sm.VertexCount} " +
                    $"IBStart={sm.IBStart} IdxCount={sm.IndexCount}");

                if (sm.IBRef >= _data.IndexBuffers.Count)
                {
                    Log($"  [SKIP] IBRef={sm.IBRef} out of range (have {_data.IndexBuffers.Count})");
                    continue;
                }
                if (sm.VBRef >= _data.Layouts.Count)
                {
                    Log($"  [SKIP] VBRef={sm.VBRef} out of range layouts (have {_data.Layouts.Count})");
                    continue;
                }

                var ib = _data.IndexBuffers[sm.IBRef];
                var layout = _data.Layouts[sm.VBRef];
                var palette = (sm.BoneMapIndex < _data.BonePalettes.Count)
                    ? _data.BonePalettes[sm.BoneMapIndex] : null;

                Log($"  IB bytes={ib.Data.Length} step={ib.Step}  total_indices={ib.Data.Length / Math.Max(ib.Step, 1)}");
                Log($"  Layout bufRefs=[{string.Join(",", layout.BufferIndices)}] semantics={layout.Semantics.Count}");

                uint ibEnd = sm.IBStart + sm.IndexCount;
                int ibTotal = ib.Data.Length / Math.Max(ib.Step, 1);
                if (ibEnd > ibTotal)
                {
                    Log($"  [WARN] IB range [{sm.IBStart}..{ibEnd}) exceeds buffer size {ibTotal} — clamping");
                    ibEnd = (uint)ibTotal;
                }

                var rawIndices = ib.GetIndices(sm.IBStart, ibEnd - sm.IBStart);
                Log($"  raw indices fetched: {rawIndices.Length}  min={(rawIndices.Length > 0 ? rawIndices.Min() : 0)}  max={(rawIndices.Length > 0 ? rawIndices.Max() : 0)}");

                var mesh = new GenericMesh
                {
                    Name = $"Submesh_{sm.ID}",
                    MaterialName = $"Material_{sm.MaterialIndex}",
                    PrimitiveType = PrimitiveType.Triangles
                };

                foreach (var idx in rawIndices)
                {
                    uint relative = (idx >= sm.VBStart) ? idx - sm.VBStart : idx;
                    mesh.Triangles.Add(relative);
                }

                int vertsBuilt = 0;
                int vertsSkipped = 0;

                var semStats = new Dictionary<string, int>();

                for (int i = 0; i < (int)sm.VertexCount; i++)
                {
                    int vGlobalIdx = (int)sm.VBStart + i;

                    var v = new GenericVertex
                    {
                        Clr = Vector4.One,
                        Weights = new Vector4(1, 0, 0, 0)
                    };

                    bool posFound = false;

                    foreach (var sem in layout.Semantics)
                    {
                        if (sem.BufIdx >= layout.BufferIndices.Count)
                        {
                            if (i == 0) Log($"  [WARN] sem.BufIdx={sem.BufIdx} >= bufRefs.Count={layout.BufferIndices.Count}");
                            continue;
                        }

                        int realVbIdx = (int)layout.BufferIndices[sem.BufIdx];
                        if (realVbIdx >= _data.VertexBuffers.Count)
                        {
                            if (i == 0) Log($"  [WARN] realVbIdx={realVbIdx} >= VB.Count={_data.VertexBuffers.Count}");
                            continue;
                        }

                        var vb = _data.VertexBuffers[realVbIdx];
                        int offset = vGlobalIdx * vb.Stride + sem.Offset;

                        if (i == 0)
                        {
                            Log($"  sem={sem.Type}[{sem.Layer}] fmt={sem.Format} vb={realVbIdx} " +
                                $"stride={vb.Stride} offset_in_buf={offset} buf_len={vb.Data.Length}");
                        }

                        if (offset < 0 || offset + 4 > vb.Data.Length)
                        {
                            if (i == 0)
                                Log($"  [WARN] offset {offset} out of VB range {vb.Data.Length}");
                            continue;
                        }

                        string semKey = $"{sem.Type}[{sem.Layer}]";
                        semStats.TryAdd(semKey, 0);
                        semStats[semKey]++;

                        switch (sem.Type)
                        {
                            case G1MSemanticType.POSITION:
                                v.Pos = vb.ReadVec3(offset, sem.Format);
                                posFound = true;
                                break;

                            case G1MSemanticType.NORMAL:
                                v.Nrm = vb.ReadVec3(offset, sem.Format);
                                break;

                            case G1MSemanticType.TEXCOORD:
                                if (sem.Layer == 0) 
                                    v.UV0 = vb.ReadVec2(offset, sem.Format);
                                break;

                            case G1MSemanticType.BLENDWEIGHT:
                                v.Weights = vb.ReadVec4(offset, sem.Format);
                                break;

                            case G1MSemanticType.BLENDINDICES:
                                var rawBI = vb.ReadVec4(offset, sem.Format);
                                v.Bones = Remap(rawBI, palette);
                                break;

                            case G1MSemanticType.COLOR:
                                if (sem.Layer == 0) v.Clr = vb.ReadVec4(offset, sem.Format);
                                break;
                        }
                    }

                    if (!posFound && i == 0)
                        Log($"  [WARN] POSITION semantic not found for any vertex in this submesh!");

                    if (i == 0)
                        Log($"  v[0] Pos={v.Pos} Nrm={v.Nrm} UV={v.UV0} W={v.Weights} B={v.Bones}");
                    if (i == 1)
                        Log($"  v[1] Pos={v.Pos}");

                    if (v.Pos == Vector3.Zero && !posFound)
                        vertsSkipped++;
                    else
                        vertsBuilt++;

                    mesh.Vertices.Add(v);
                }

                Log($"  verts built={vertsBuilt} skipped(no pos)={vertsSkipped}");
                Log($"  semStats: {string.Join(" | ", semStats.Select(kv => $"{kv.Key}={kv.Value}"))}");
                Log($"  triangles: {mesh.Triangles.Count}  (tris={mesh.Triangles.Count / 3})");

                if (mesh.Vertices.Count > 0 && mesh.Triangles.Count > 0)
                {
                    model.Meshes.Add(mesh);
                    meshesAdded++;
                    Log($"  [OK] mesh added");
                }
                else
                {
                    Log($"  [SKIP] mesh empty — verts={mesh.Vertices.Count} tris={mesh.Triangles.Count}");
                }
            }

            Log($"\n[ToGenericModel] done — meshes={meshesAdded}/{_data.Submeshes.Count}");

            return model;
        }

        // ─────────────────────────────────────────────────────────────
        private Vector4 Remap(Vector4 raw, List<uint> palette)
        {
            if (palette == null)
            {
                return new Vector4(raw.X / 3, raw.Y / 3, raw.Z / 3, raw.W / 3);
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
    }
}