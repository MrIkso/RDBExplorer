namespace RDBExplorer.Core.Formats.LayeredFile
{
    public class LFMInfEntry
    {
        public int Index { get; set; }
        public int UnCompressedSize { get; set; }
        public int IsCompressed { get; set; }

    }

    public class LFMInfFile
    {
        public int FileCount { get; set; }
        public List<LFMInfEntry> Files { get; set; }
    }

    internal class LFMInfFileParser
    {
        public LFMInfFile Read(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                LFMInfFile infFile = new LFMInfFile();

                infFile.FileCount = reader.ReadInt32();

                var list = new List<LFMInfEntry>();
                for (int i = 0; i < infFile.FileCount; i++)
                {
                    LFMInfEntry entry = new LFMInfEntry();
                    entry.Index = reader.ReadInt32();
                    entry.UnCompressedSize = reader.ReadInt32();
                    entry.IsCompressed = reader.ReadInt32();

                    list.Add(entry);
                }
                infFile.Files = list;
                return infFile;
            }
        }

        public void Write(List<LFMInfEntry> lFMInfEntries, string savePath)
        {
            using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                writer.Write(lFMInfEntries.Count);

                if (lFMInfEntries != null)
                {
                    foreach (var entry in lFMInfEntries)
                    {
                        writer.Write(entry.Index);
                        writer.Write(entry.UnCompressedSize);
                        writer.Write(entry.IsCompressed);
                    }
                }
            }
        }
    }
}