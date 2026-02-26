using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.SID
{
    public class SidParser
    {
        public SIDFile SIDFile { get; set; }

        public void Load(byte[] data)
        {
            SIDFile = new SIDFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                SIDHeader header = br.ReadStruct<SIDHeader>();
                SIDFile.Header = header;
                
                for (int i = 0; i < header.EntriesCount; i++)
                {
                    var entry = br.ReadStruct<SIDEntry>();
                    SIDFile.Entries.Add(entry);
                }
            }
        }
    }
}
