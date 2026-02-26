using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.GRBF
{
    public class GRBFParser
    {
        public GRBFFile File { get; set; }

        public void Load(byte[] data)
        {
            File = new GRBFFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                GRBFHeader header = new GRBFHeader();
                header.Magic = br.ReadEncodedString(4);
                header.Version = br.ReadEncodedString(4);
                header.BlockSize = br.ReadUInt32();
                header.EntriesCount = br.ReadUInt32();
                
                File.Header = header;

                for (int i = 0; i < header.EntriesCount; i++)
                {
                    GRBFEntry entry = new GRBFEntry();
                    entry.BlockSize = br.ReadUInt32();

                    int elementCount = (int)((entry.BlockSize - 4) / 4);

                    uint[] dataElements = new uint[elementCount];
                    for (int j = 0; j < elementCount; j++)
                    {
                        dataElements[j] = br.ReadUInt32();
                    }
                    entry.Data = dataElements;

                    File.Entries.Add(entry);
                }
            }
        }
    }
}
