using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.KTS
{
    public struct KTSEntry
    {
        public ushort Unk1 { get; set; }
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public ushort Unk4 { get; set; }
        public ushort Unk5 { get; set; }
        public ushort Unk6 { get; set; }
    }

    public class KTSFile
    {
        public string Magic { get; set; }
        public uint Version { get; set; }
        public uint EntriesCount { get; set; }
        public ushort Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public List<KTSEntry> Entries { get; set; } = new List<KTSEntry>();
    }

    public class KTSParser
    {
        public KTSFile KTSFile { get; set; }
        
        public void Load(byte[] data)
        {
            KTSFile = new KTSFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                KTSFile.Magic = br.ReadEncodedString(4);
                KTSFile.Version = br.ReadUInt32();
                KTSFile.EntriesCount = br.ReadUInt16();
                KTSFile.Unk1 = br.ReadUInt16();
                KTSFile.Unk2 = br.ReadUInt32();

                for (int i = 0; i < KTSFile.EntriesCount; i++)
                {
                    var entry = br.ReadStruct<KTSEntry>();
                    KTSFile.Entries.Add(entry);
                }
            }
        }
    }
}
