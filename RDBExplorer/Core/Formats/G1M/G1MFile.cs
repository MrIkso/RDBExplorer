using OpenTK.Mathematics;
using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.G1M
{
    internal class G1MData
    {
        public List<G1MBoneInternal> Skeleton = new List<G1MBoneInternal>();
        public List<G1MVertexBufferInternal> VertexBuffers = new List<G1MVertexBufferInternal>();
        public List<G1MIndexBufferInternal> IndexBuffers = new List<G1MIndexBufferInternal>();
        public List<G1MLayoutInternal> Layouts = new List<G1MLayoutInternal>();
        public List<G1MSubmeshInternal> Submeshes = new List<G1MSubmeshInternal>();
        public List<List<uint>> BonePalettes = new List<List<uint>>();
        public Dictionary<int, int> LocalToGlobalBone = new Dictionary<int, int>();
        public List<List<uint>> ResolvedPalettes = new List<List<uint>>();
        public List<G1MMaterialInternal> Materials = new();
        public List<G1MMeshGroupInternal> MeshGroups = new();

        public void Parse(BinaryReader r)
        {
            r.ReadUInt32(); // Magic
            r.ReadUInt32(); // Version
            r.ReadUInt32(); // FileSize
            uint headerOffset = r.ReadUInt32();
            r.ReadUInt32();
            uint chunkCount = r.ReadUInt32();

            r.BaseStream.Position = headerOffset;

            for (int i = 0; i < chunkCount; i++)
            {
                long start = r.BaseStream.Position;
                uint magic = r.ReadUInt32();
                uint version = r.ReadUInt32();
                uint size = r.ReadUInt32();

                if (magic == 0x47314D53)
                    ParseG1MS(r, start);          // G1MS
                else if (magic == 0x47314D47)
                    ParseG1MG(r, start, version); // G1MG

                r.BaseStream.Position = start + size;
            }
        }

        
        // G1MS  –  Skeleton
        
        private void ParseG1MS(BinaryReader r, long start)
        {
            // Header layout (after magic/version/size that were already read):
            //   +0x00  jointDataOffset  (uint)
            //   +0x04  unknown          (uint)
            //   +0x08  jointCount       (ushort)
            //   +0x0A  jointIndicesCount(ushort)
            //   +0x0C  layer            (ushort)
            //   +0x0E  pad              (ushort)
            //   +0x10  boneIDList[jointIndicesCount]  (ushort each)
            r.BaseStream.Position = start + 0x0C; // skip magic(4)+version(4)+size(4)
            uint jointDataOffset = r.ReadUInt32();
            r.ReadUInt32(); // unknown
            ushort boneCount = r.ReadUInt16();
            ushort jointIndicesCount = r.ReadUInt16();
            r.ReadUInt16(); // layer
            r.ReadUInt16(); // pad

            // Read boneIDList (used to map joint index → global bone ID)
            var boneIDList = new ushort[jointIndicesCount];
            var boneToBoneID = new Dictionary<int, int>(); // boneID → list index
            for (int i = 0; i < jointIndicesCount; i++)
            {
                boneIDList[i] = r.ReadUInt16();
                if (boneIDList[i] != 0xFFFF)
                    boneToBoneID[boneIDList[i]] = i;
            }

            // Read joint data
            r.BaseStream.Position = start + jointDataOffset;
            for (int i = 0; i < boneCount; i++)
            {
                // scale  (3 × float  = 12 bytes)
                float sx = r.ReadSingle();
                float sy = r.ReadSingle();
                float sz = r.ReadSingle();

                // parentID  (int  = 4 bytes)
                int parentIndex = r.ReadInt32();

                // rotation quaternion stored as x,y,z,w  (4 × float = 16 bytes)
                float qx = r.ReadSingle();
                float qy = r.ReadSingle();
                float qz = r.ReadSingle();
                float qw = r.ReadSingle();

                // position x,y,z,w  (4 × float = 16 bytes); w is padding
                float px = r.ReadSingle();
                float py = r.ReadSingle();
                float pz = r.ReadSingle();
                r.ReadSingle(); // w – padding, ignored

                // OpenTK Quaternion constructor is (x, y, z, w)
                Skeleton.Add(new G1MBoneInternal
                {
                    Name = $"Bone_{i}",
                    Scale = new Vector3(sx, sy, sz),
                    ParentIndex = parentIndex,
                    Rotation = new Quaternion(qx, qy, qz, qw),
                    Position = new Vector3(px, py, pz)
                });
            }
        }

        
        // G1MG  –  Geometry
        
        private void ParseG1MG(BinaryReader r, long start, uint version)
        {
            // After magic(4)+version(4)+size(4) = 0x0C:
            //   platform  (4 bytes)
            //   reserved  (4 bytes)
            //   bounding box 6×float (24 bytes)
            //   sectionCount (4 bytes)
            // Total skip from start = 0x0C + 4 + 4 + 24 + 4 = 0x0C + 0x20 = 0x2C,
            // but we need to READ sectionCount, so position = start + 0x0C + 0x20
            r.BaseStream.Position = start + 0x0C + 0x20;
            uint sectionCount = r.ReadUInt32();

            for (int i = 0; i < sectionCount; i++)
            {
                long secStart = r.BaseStream.Position;
                uint magic = r.ReadUInt32();
                uint size = r.ReadUInt32();
                uint count = r.ReadUInt32();

                switch (magic)
                {
                    case 0x00010002: ParseMaterials(r, count); break;
                    case 0x00010004: ParseVertexBuffers(r, count, version); break;
                    case 0x00010005: ParseVertexAttributes(r, count); break;
                    case 0x00010006: ParseJointPalettes(r, count); break;
                    case 0x00010007: ParseIndexBuffers(r, count, version); break;
                    case 0x00010008: ParseSubmeshes(r, count); break;
                     case 0x00010009: ParseMeshGroups(r, count, version); break;
                    default:
                        Console.WriteLine($"unhandle magic: 0x{magic:X08}");
                        break;
                }

                r.BaseStream.Position = secStart + size;
            }
        }

        private void ParseMaterials(BinaryReader r, uint count)
        {
            for (int j = 0; j < (int)count; j++)
            {
                r.ReadUInt32(); // unk1
                uint texCount = r.ReadUInt32();
                r.ReadUInt32(); // unk
                r.ReadUInt32(); // unk

                var mat = new G1MMaterialInternal();
                for (int k = 0; k < (int)texCount; k++)
                {
                    mat.Textures.Add(new G1MTextureRef
                    {
                        Index = r.ReadUInt16(),
                        Layer = r.ReadUInt16(),
                        TextureType = r.ReadUInt16(),
                    });
                    r.ReadUInt16(); // otherType
                    r.ReadUInt16(); // tileX
                    r.ReadUInt16(); // tileY
                }
                Materials.Add(mat);
            }
        }

        
        // 0x00010004  –  Vertex Buffers  (segmented)
        
        private void ParseVertexBuffers(BinaryReader r, uint count, uint version)
        {
            int total = 0;

            while (total < (int)count)
            {
                // Each entry: unknown1(4) + stride(4) + vCount(4) [+ extra(4) if version > 0x30303430]
                r.ReadUInt32(); // unknown1  (NOT a flag – any value is valid here)
                int stride = r.ReadInt32();
                int vCount = r.ReadInt32();
                if (version > 0x30303430)
                    r.ReadUInt32();

                byte[] physData;
                int physStride;

                if (stride == 1)
                {
                    // This buffer is a raw repository; actual data for sub-buffers lives here.
                    physStride = 1;
                    physData = r.ReadBytes(vCount);
                }
                else
                {
                    physStride = stride;
                    physData = r.ReadBytes(stride * vCount);
                }

                VertexBuffers.Add(new G1MVertexBufferInternal(physData, physStride));
                total++;

                // Peek for segmented sub-buffers flagged by unknown1 == 0x80000000
                int accOffset = 0;
                while (r.BaseStream.Position + 4 <= r.BaseStream.Length)
                {
                    long peekPos = r.BaseStream.Position;
                    uint flag = r.ReadUInt32();
                    if (flag != 0x80000000)
                    {
                        r.BaseStream.Position = peekPos; // put it back
                        break;
                    }

                    int subStride = r.ReadInt32();
                    int subCount = r.ReadInt32();
                    if (version > 0x30303430)
                        r.ReadUInt32();

                    int subBytes = subStride * subCount;
                    byte[] sub = new byte[subBytes];
                    int avail = physData.Length - accOffset;
                    Array.Copy(physData, accOffset, sub, 0, Math.Min(subBytes, avail));
                    accOffset += subBytes;

                    VertexBuffers.Add(new G1MVertexBufferInternal(sub, subStride));
                    total++;
                }
            }
        }

        
        // 0x00010005  –  Vertex Attributes (Layouts)
        
        private void ParseVertexAttributes(BinaryReader r, uint count)
        {
            for (int j = 0; j < (int)count; j++)
            {
                var layout = new G1MLayoutInternal();

                // 1. Buffer reference list (indirect addressing)
                uint numRefs = r.ReadUInt32();
                for (int k = 0; k < (int)numRefs; k++)
                    layout.BufferIndices.Add(r.ReadUInt32());

                // 2. Semantic descriptors – each is exactly 8 bytes:
                //    bufferID (ushort=2) | offset (ushort=2) | dataType (byte=1) | dummy (byte=1) | semantic (byte=1) | layer (byte=1)
                uint numSemantics = r.ReadUInt32();
                for (int k = 0; k < (int)numSemantics; k++)
                {
                    ushort bufIdx = r.ReadUInt16(); // which buffer in BufferIndices
                    ushort offset = r.ReadUInt16(); // byte offset within that buffer's stride
                    G1MDataFormat dataType = (G1MDataFormat)r.ReadByte();   //
                    r.ReadByte();                      // dummy – ignored
                    byte semantic = r.ReadByte();   // semantic type enum
                    byte layer = r.ReadByte();   // semantic index / layer

                    layout.Semantics.Add(new G1MSemanticInternal
                    {
                        BufIdx = bufIdx,
                        Offset = offset,
                        Format = dataType,
                        RawType = semantic,
                        Layer = layer
                    });
                }

                Layouts.Add(layout);
            }
        }

        
        // 0x00010006  –  Joint Palettes
        
        private void ParseJointPalettes(BinaryReader r, uint count)
        {
            for (int i = 0; i < count; i++)
            {
                uint pCount = r.ReadUInt32();
                var palette = new List<uint>();
                for (int j = 0; j < pCount; j++)
                {
                    r.ReadUInt32(); // G1MM index
                    r.ReadUInt32(); // physics index
                    uint jointIdx = r.ReadUInt32();

                    // Логіка 0x80000000 з C++ та Python
                    uint actualIdx = jointIdx;
                    if ((jointIdx & 0x80000000) != 0)
                        actualIdx ^= 0x80000000;

                    if (LocalToGlobalBone.TryGetValue((int)actualIdx, out int globalID))
                        palette.Add((uint)globalID);
                    else
                        palette.Add(actualIdx);
                }
                ResolvedPalettes.Add(palette);
            }
        }
        
        // 0x00010007  –  Index Buffers
        
        private void ParseIndexBuffers(BinaryReader r, uint count, uint version)
        {
            for (int j = 0; j < (int)count; j++)
            {
                uint iCount = r.ReadUInt32();
                uint iType = r.ReadUInt32(); // bit-width: 16 → 2 bytes, 32 → 4 bytes
                if (version > 0x30303430)
                    r.ReadUInt32();

                int byteWidth = (int)(iType / 8); // 16/8=2, 32/8=4
                if (byteWidth < 1)
                    byteWidth = 2;  // safety fallback

                byte[] data = r.ReadBytes((int)iCount * byteWidth);
                IndexBuffers.Add(new G1MIndexBufferInternal(data, byteWidth));

                // Align to 4 bytes
                if (r.BaseStream.Position % 4 != 0)
                    r.BaseStream.Position += 4 - (r.BaseStream.Position % 4);
            }
        }

        
        // 0x00010008  –  Submeshes
        
        private void ParseSubmeshes(BinaryReader r, uint count)
        {
            for (int i = 0; i < count; i++)
            {
                r.ReadUInt32(); // flags
                int vbIdx = r.ReadInt32();
                int palIdx = r.ReadInt32();
                r.ReadUInt32(); r.ReadUInt32(); r.ReadUInt32(); // unks
                int matIdx = r.ReadInt32();
                int ibIdx = r.ReadInt32();
                r.ReadUInt32(); // unk
                uint prim = r.ReadUInt32();
                uint vbStart = r.ReadUInt32();
                uint vCount = r.ReadUInt32();
                uint ibStart = r.ReadUInt32();
                uint iCount = r.ReadUInt32();

                Submeshes.Add(new G1MSubmeshInternal
                {
                    ID = i,
                    VBRef = vbIdx,
                    BoneMapIndex = palIdx,
                    MaterialIndex = matIdx,
                    IBRef = ibIdx,
                    VBStart = vbStart,
                    VertexCount = vCount,
                    IBStart = ibStart,
                    IndexCount = iCount,
                    PrimType = prim
                });
            }
        }


        private void ParseMeshGroups(BinaryReader r, uint count, uint version)
        {
            for (int j = 0; j < (int)count; j++)
            {
                var group = new G1MMeshGroupInternal();

                if (version > 0x30303330)
                {
                    group.LOD = r.ReadUInt32();
                    group.Group = r.ReadUInt32();
                    r.ReadUInt32(); // GroupEntryIndex
                    uint sm1 = r.ReadUInt32(); // submeshCount1 (type 53)
                    uint sm2 = r.ReadUInt32(); // submeshCount2 (type 61)

                    if (version > 0x30303430)
                    {
                        r.ReadUInt32(); // lodRangeStart
                        r.ReadUInt32(); // lodRangeLength
                        r.ReadUInt32(); r.ReadUInt32(); // padding
                    }

                    for (int k = 0; k < (int)(sm1 + sm2); k++)
                        group.Meshes.Add(ReadMesh(r));
                }
                else
                {
                    group.LOD = r.ReadUInt32();
                    uint sm1 = r.ReadUInt32();
                    uint sm2 = r.ReadUInt32();
                    for (int k = 0; k < (int)(sm1 + sm2); k++)
                        group.Meshes.Add(ReadMesh(r));
                }

                MeshGroups.Add(group);
            }
        }

        private G1MMeshInternal ReadMesh(BinaryReader r)
        {
            var mesh = new G1MMeshInternal();
            string name = r.ReadEncodedString(16);
          //  Console.WriteLine(name);
            mesh.ClothID = r.ReadUInt16();
            r.ReadUInt16();                       // unk
            mesh.ExternalID = r.ReadUInt32();
            uint idxCount = r.ReadUInt32();

            if (idxCount > 0)
            {
                for (int i = 0; i < (int)idxCount; i++)
                    mesh.SubmeshIndices.Add(r.ReadUInt32());
            }
            else
                r.ReadUInt32();

            return mesh;
        }

    }
}

// Vertex Buffer helper
internal class G1MVertexBufferInternal
{
    public byte[] Data;
    public int Stride;

    public G1MVertexBufferInternal(byte[] d, int s) { Data = d; Stride = s; }

    public Vector3 ReadVec3(int o, G1MDataFormat fmt)
    {
        if (fmt == G1MDataFormat.R16G16_FLOAT || fmt == G1MDataFormat.R16G16B16A16_FLOAT)
            return new Vector3(ReadHalf(o), ReadHalf(o + 2), ReadHalf(o + 4));

        if (fmt == G1MDataFormat.R16G16B16A16_UINT) // R16G16B16A16
            return new Vector3(
                BitConverter.ToInt16(Data, o),
                BitConverter.ToInt16(Data, o + 2),
                BitConverter.ToInt16(Data, o + 4));

        return new Vector3(ReadFloat(o), ReadFloat(o + 4), ReadFloat(o + 8));
    }

    public Vector2 ReadVec2(int o, G1MDataFormat fmt)
    {
        if (fmt == G1MDataFormat.R16G16_FLOAT || fmt == G1MDataFormat.R16G16B16A16_FLOAT)
            return new Vector2(ReadHalf(o), ReadHalf(o + 2));
        return new Vector2(ReadFloat(o), ReadFloat(o + 4));
    }

    public Vector4 ReadVec4(int o, G1MDataFormat fmt)
    {
        switch (fmt)
        {
            case G1MDataFormat.R8G8B8A8_UINT:  // R8G8B8A8_UINT  – raw byte values
            case G1MDataFormat.R16G16B16A16_UINT:  // R16G16B16A16_UINT (treated as bytes for indices)
                return new Vector4(Data[o], Data[o + 1], Data[o + 2], Data[o + 3]);

            case G1MDataFormat.R8G8B8A8_UNORM: // R8G8B8A8_UNORM  – normalised [0,1]
                return new Vector4(Data[o] / 255f, Data[o + 1] / 255f, Data[o + 2] / 255f, Data[o + 3] / 255f);

            case G1MDataFormat.R16G16B16A16_FLOAT: // R16G16B16A16_FLOAT
                return new Vector4(ReadHalf(o), ReadHalf(o + 2), ReadHalf(o + 4), ReadHalf(o + 6));

            default: // R32G32B32A32_FLOAT
                return new Vector4(ReadFloat(o), ReadFloat(o + 4), ReadFloat(o + 8), ReadFloat(o + 12));
        }
    }

    private float ReadFloat(int o)
        => (o + 4 <= Data.Length) ? BitConverter.ToSingle(Data, o) : 0f;

    private float ReadHalf(int o)
    {
        if (o + 2 > Data.Length)
            return 0f;
        ushort h = BitConverter.ToUInt16(Data, o);
        int s = (h >> 15) & 0x01;
        int e = (h >> 10) & 0x1F;
        int m = h & 0x3FF;
        float sign = (s == 1) ? -1f : 1f;
        if (e == 0)
            return sign * (float)(Math.Pow(2, -14) * (m / 1024.0));
        if (e == 31)
            return m == 0 ? (sign * float.PositiveInfinity) : float.NaN;
        return sign * (float)(Math.Pow(2, e - 15) * (1.0 + m / 1024.0));
    }
}

// Index Buffer helper

internal class G1MIndexBufferInternal
{
    public byte[] Data;
    public int Step; // bytes per index (2 or 4)

    public G1MIndexBufferInternal(byte[] d, int s) { Data = d; Step = s; }

    public uint[] GetIndices(uint start, uint count)
    {
        var res = new uint[count];
        for (int i = 0; i < (int)count; i++)
        {
            int o = ((int)start + i) * Step;
            if (o + Step > Data.Length) break;
            res[i] = (Step == 4)
                ? BitConverter.ToUInt32(Data, o)
                : BitConverter.ToUInt16(Data, o);
        }
        return res;
    }
}


// Plain data classes

internal class G1MBoneInternal
{
    public string Name;
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
    public int ParentIndex;
}

internal class G1MSubmeshInternal
{
    public int ID, VBRef, IBRef, BoneMapIndex, MaterialIndex;
    public uint VBStart, VertexCount, IBStart, IndexCount, PrimType;
}

internal class G1MLayoutInternal
{
    public List<uint> BufferIndices = new List<uint>();
    public List<G1MSemanticInternal> Semantics = new List<G1MSemanticInternal>();
}

internal class G1MSemanticInternal
{
    public ushort BufIdx;   // index into layout's BufferIndices list
    public ushort Offset;   // byte offset within the buffer stride
    public G1MDataFormat Format;   // data format enum  (see G1MDataFormat)
    public byte RawType;  // semantic enum     (see G1MSemanticType)
    public byte Layer;    // semantic index / UV layer etc.

    public G1MSemanticType Type => (G1MSemanticType)RawType;
    public int Index => Layer;
}

internal class G1MTextureRef
{
    public ushort Index;       // індекс в G1T
    public ushort Layer;       // TEXCOORD layer
    public ushort TextureType; // 0=diffuse, 1=normal, 2=specular...
}

internal class G1MMaterialInternal
{
    public List<G1MTextureRef> Textures = new();
}
internal class G1MMeshInternal
{
    public ushort ClothID;
    public uint ExternalID;
    public List<uint> SubmeshIndices = new(); // індекси в Submeshes[]
}

internal class G1MMeshGroupInternal
{
    public uint LOD;
    public uint Group;
    public List<G1MMeshInternal> Meshes = new();
}

internal enum G1MSemanticType
{
    POSITION = 0,
    BLENDWEIGHT = 1,
    BLENDINDICES = 2,
    NORMAL = 3,
    PSIZE = 4,
    TEXCOORD = 5,
    TANGENT = 6,
    BINORMAL = 7,
    TESSFACTOR = 8,
    POSITIONT = 9,
    COLOR = 10,
    FOG = 11,
    DEPTH = 12,
    SAMPLE = 13
}

internal enum G1MDataFormat : byte
{
    R32_FLOAT = 0,
    R32G32_FLOAT = 1,
    R32G32B32_FLOAT = 2,
    R32G32B32A32_FLOAT = 3,
    R8G8B8A8_UINT = 5,
    R16G16B16A16_UINT = 7,
    R32G32B32A32_UINT = 10,
    R16G16_FLOAT = 10,
    R16G16B16A16_FLOAT = 11,
    R8G8B8A8_UNORM = 13
}
