using RDBExplorer.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RDBExplorer.Core.Formats.G1CO
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Node
    {
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public float Unk4 { get; set; }
        public float Unk5 { get; set; }
        public float Unk6 { get; set; }
        public float Unk7 { get; set; }
        public float Unk8 { get; set; }
        public uint Unk9 { get; set; }
        public uint Unk10 { get; set; }
        public uint Unk11 { get; set; }
        public uint Unk12 { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Extra
    {
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public uint Unk4 { get; set; }
    }

    public class G1COHeader
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public byte Unk { get; set; }
        public byte MetadataSize { get; set; }
        public byte NumEntries { get; set; }
        public byte DispatchMode { get; set; }
        public uint FileSize { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Triangle
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Z { get; set; }
}

    public class HVB
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint Unk1 { get; set; }
        public uint BlockSize { get; set; }
        public uint NodesCount { get; set; }
        public uint NodesOffset { get; set; }
        public uint TriangleCount { get; set; }
        public uint TriangleOffset { get; set; }
        public uint ExtraCount { get; set; }
        public uint ExtraOffset { get; set; }
        public uint Pad { get; set; }
        public uint Unk2 { get; set; }

        public Node[] Nodes { get; set; }
        public Triangle[] Triangles { get; set; }
        public Extra[] ExtraData { get; set; }


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
                Triangles = new Triangle[TriangleCount];
                for (int i = 0; i < TriangleCount; i++)
                {
                    Triangles[i] = br.ReadStruct<Triangle>();
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

    public class G1COFile
    {
        public G1COHeader Header { get; set; }
        public List<HVB> HVBs { get; set; } = new List<HVB>();
    }
}
