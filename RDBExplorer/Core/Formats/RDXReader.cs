using RDBExplorer.Core.Models;

namespace RDBExplorer.Core.Formats
{
    public class RDXReader
    {
        public List<RdxEntry> RdxEntries { get; set; }

        public RDXReader(string rdxFilePath) {
            RdxEntries = Read(rdxFilePath);
        }

        public List<RdxEntry> Read(string filePath)
        {
            var entries = new List<RdxEntry>();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    entries.Add(new RdxEntry
                    {
                        Index = reader.ReadUInt16(),
                        Marker = reader.ReadUInt16(),
                        FileId = reader.ReadUInt32()
                    });
                }
            }
            return entries;
        }
    }
}
