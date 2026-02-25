using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.MTL
{
    public class MTLParser
    {
        public MTLFile MTLFile { get; set; }

        public void Load(byte[] data)
        {
            MTLFile = new MTLFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                MTLHeader header = br.ReadStruct<MTLHeader>();
                MTLFile.Header = header;
                List<MaterialEntry> materialEntries = new List<MaterialEntry>();
                List<PhysicsEntry> clothsEntries = new List<PhysicsEntry>();
                List<PhysicsEntry> ponyTailsEntries = new List<PhysicsEntry>();

                for (int i = 0; i < header.NumNames; i++)
                {
                    MaterialEntry entry = new MaterialEntry();
                    entry.NameKTID = br.ReadUInt32();
                    entry.IdCount = br.ReadUInt32();
                    entry.Ids = new uint[entry.IdCount];
                    for (int j = 0; j < entry.IdCount; j++)
                    {
                        entry.Ids[j] = br.ReadUInt32();
                    }
                    materialEntries.Add(entry);
                }

                for (int i = 0; i < header.NumCloths; i++)
                {
                    PhysicsEntry entry = br.ReadStruct<PhysicsEntry>();
                    clothsEntries.Add(entry);
                }

                for (int i = 0; i < header.NumPonyTails; i++)
                {
                    PhysicsEntry entry = br.ReadStruct<PhysicsEntry>();
                    ponyTailsEntries.Add(entry);
                }
                MTLFile.MaterialEntries = materialEntries;
                MTLFile.ClothsEntries = clothsEntries;
                MTLFile.PonyTailsEntries = ponyTailsEntries;
            }
        }
    }
}
