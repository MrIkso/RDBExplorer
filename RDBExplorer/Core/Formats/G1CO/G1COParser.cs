using RDBExplorer.Utils;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RDBExplorer.Core.Formats.G1CO
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Node
    {
        public float Unk1, Unk2, Unk3, Unk4;
        public float Unk5, Unk6, Unk7, Unk8;
        public uint Unk9, Unk10, Unk11, Unk12;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Triangle
    {
        public ushort X, Y, Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Extra
    {
        public float Unk1, Unk2, Unk3;
        public uint Unk4;
    }

    public class G1COHeader
    {
        public string Magic;
        public string Version;
        public byte Unk;
        public byte MetadataSize;
        public byte NumEntries;
        public byte DispatchMode;
        public uint FileSize;
    }

    public class G1COFile
    {
        public G1COHeader Header { get; set; }
        public List<HVB> HVBs { get; set; } = new List<HVB>();

        public void Load(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms, Encoding.ASCII))
            {
                Header = new G1COHeader
                {
                    Magic = br.ReadEncodedString(4),
                    Version = br.ReadEncodedString(4),
                    Unk = br.ReadByte(),
                    MetadataSize = br.ReadByte(),
                    NumEntries = br.ReadByte(),
                    DispatchMode = br.ReadByte(),
                    FileSize = br.ReadUInt32()
                };

                if (Header.Magic != "OC1G")
                    throw new Exception("Invalig G1CO magic");

                if (Header.MetadataSize > 0)
                {
                    br.BaseStream.Position += Header.MetadataSize;
                }

                for (int i = 0; i < Header.NumEntries; i++)
                {
                    HVB hvb = new();
                    hvb.Read(br);
                    HVBs.Add(hvb);
                }
            }
        }
    }

    public class HVB
    {
        public string Magic;
        public string Version;
        public uint Unk1;
        public uint BlockSize;
        public uint NodesCount;
        public uint NodesOffset;
        public uint TriangleCount;
        public uint TriangleOffset;
        public uint ExtraCount;
        public uint ExtraOffset;
        public uint Pad;
        public uint Unk2;

        public Node[] Nodes;
        public Vector3[] Triangles;
        public Extra[] ExtraData;

        public void Read(BinaryReader br)
        {
            long hvbStartPos = br.BaseStream.Position;

            Magic = br.ReadEncodedString(4);
            Version = br.ReadEncodedString(4);
            Unk1 = br.ReadUInt32();
            BlockSize = br.ReadUInt32();
            NodesCount = br.ReadUInt32();
            NodesOffset = br.ReadUInt32();
            TriangleCount = br.ReadUInt32();
            TriangleOffset = br.ReadUInt32();
            ExtraCount = br.ReadUInt32();
            ExtraOffset = br.ReadUInt32();
            Pad = br.ReadUInt32();
            Unk2 = br.ReadUInt32();

            if (NodesCount > 0)
            {
                br.BaseStream.Position = hvbStartPos + NodesOffset;
                Nodes = new Node[NodesCount];
                for (int i = 0; i < NodesCount; i++)
                {
                    Nodes[i] = br.ReadStruct<Node>();
                }
            }

            if (TriangleCount > 0)
            {
                br.BaseStream.Position = hvbStartPos + TriangleOffset;
                Triangles = new Vector3[TriangleCount];
                for (int i = 0; i < TriangleCount; i++)
                {
                    Triangles[i] = br.ReadStruct<Vector3>();
                }
            }

            if (ExtraCount > 0)
            {
                br.BaseStream.Position = hvbStartPos + ExtraOffset;
                ExtraData = new Extra[ExtraCount];
                for (int i = 0; i < ExtraCount; i++)
                {
                    ExtraData[i] = br.ReadStruct<Extra>();
                }
            }

            br.BaseStream.Position = hvbStartPos + BlockSize;
        }
    }
}
