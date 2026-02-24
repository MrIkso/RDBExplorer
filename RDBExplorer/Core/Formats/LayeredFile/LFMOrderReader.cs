using RDBExplorer.Utils;
using System.Text;

namespace RDBExplorer.Core.Formats.LayeredFile
{
    public struct LFMOrderHeader
    {
        public uint Magic;
        public uint Version;
        public uint FileCount;
        public uint HeaderSize;
        public uint DataPointer;
        public uint NameTablePointer;
        public uint FirstNamePointer;
        public int Reserved1 { get; set; }
        public int Reserved2 { get; set; }
        public int SourceNamePointer { get; set; }
    }

    public struct LFMOrderEntry
    {
        public uint Reserved;
        public uint Index;
        public uint Pointer;
    }

    public class LFMOrderReader
    {
        public List<string> HashedPathNames = new List<string>();

        public void Read(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))

            using (var reader = new BinaryReader(fs))
            {
                var header = new LFMOrderHeader();
                header.Magic = reader.ReadUInt32();
                header.Version = reader.ReadUInt32();
                header.FileCount = reader.ReadUInt32();
                header.HeaderSize = reader.ReadUInt32();
                header.DataPointer = reader.ReadUInt32();
                header.NameTablePointer = reader.ReadUInt32();
                header.FirstNamePointer = reader.ReadUInt32();
                header.Reserved1 = reader.ReadInt32();
                header.Reserved2 = reader.ReadInt32();
                header.SourceNamePointer = reader.ReadInt32();

                List<LFMOrderEntry> stringOffsets = new List< LFMOrderEntry>();

                for (int i = 0; i < header.FileCount; i++)
                {
                    var strOffset = new LFMOrderEntry();
                    strOffset.Reserved = reader.ReadUInt32();
                    strOffset.Index = reader.ReadUInt32();
                    strOffset.Pointer = reader.ReadUInt32();
                    stringOffsets.Add(strOffset);
                }

                foreach (var strOffset in stringOffsets)
                {
                    reader.BaseStream.Position = strOffset.Pointer;
                    string path = reader.ReadNullTerminatedString(Encoding.ASCII);
                    HashedPathNames.Add(path);
                }
            }
        }
    }
}
