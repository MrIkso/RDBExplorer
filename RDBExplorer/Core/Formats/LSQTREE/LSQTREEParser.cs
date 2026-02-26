using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.LSQTREE
{
    public class LSQTREEParser
    {
        public LSQTREEFile File { get; set; }

        public void Load(byte[] data)
        {
            File = new LSQTREEFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
               LSQTREEHeader header = br.ReadStruct<LSQTREEHeader>();

                for (int i = 0; i < header.EntriesCount; i++)
                {
                    var entry = br.ReadStruct<LSQTREEEntry>();
                    File.Entries.Add(entry);
                }

                File.Header = header;
            }
        }
    }
}
