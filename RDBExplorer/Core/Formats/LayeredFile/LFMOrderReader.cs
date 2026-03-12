using RDBExplorer.Utils;
using System.Text;

namespace RDBExplorer.Core.Formats.LayeredFile
{
    public struct LFMOrderHeader
    {
        public uint Magic { get; set; }
        public uint Version { get; set; }
        public uint FileCount { get; set; }
        public uint HeaderSize { get; set; }
        public uint DataPointer { get; set; }
        public uint NameTablePointer { get; set; }
        public uint FirstNamePointer { get; set; }
        public int Reserved1 { get; set; }
        public int Reserved2 { get; set; }
        public int SourceNamePointer { get; set; }
    }

    public struct LFMOrderEntry
    {
        public uint Reserved { get; set; }
        public uint Index { get; set; }
        public uint Pointer { get; set; }
    }

    public class LFMOFile
    {
        public LFMOrderHeader Header;
        public List<LFMOrderEntry> OrderEntries = new List<LFMOrderEntry>();
        public List<string> HashedPathNames = new List<string>();
    }

    public class LFMOrderReader
    {
        public LFMOFile File { get; set; }
        public List<string> HashedPathNames = new List<string>();

        public void Read(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))

            using (var reader = new BinaryReader(fs))
            {
                var header = reader.ReadStruct<LFMOrderHeader>();

                List<LFMOrderEntry> stringOffsets = new List< LFMOrderEntry>();

                for (int i = 0; i < header.FileCount; i++)
                {
                    var strOffset = reader.ReadStruct<LFMOrderEntry>();
                    stringOffsets.Add(strOffset);
                }

                foreach (var strOffset in stringOffsets)
                {
                    reader.BaseStream.Position = strOffset.Pointer;
                    string path = reader.ReadNullTerminatedString(Encoding.ASCII);
                    HashedPathNames.Add(path);
                }

                File = new LFMOFile();
                File.Header = header;
                File.OrderEntries = stringOffsets;
                File.HashedPathNames = HashedPathNames;
            }
        }
    }
}
