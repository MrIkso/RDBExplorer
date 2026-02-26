namespace RDBExplorer.Core.Formats.LSQTREE
{
    public struct LSQTREEEntry {
        public ushort Unk1 { get; set; }
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public ushort Index { get; set; }
        public uint Unk4 { get; set; }
        public uint Unk5 { get; set; }
        public ushort Unk6 { get; set; }
        public ushort Unk7 { get; set; }
        public uint KTID { get; set; }
        public uint Unk8 { get; set; }
        public uint Unk9 { get; set; }
    }

    public struct LSQTREEHeader
    {
        public uint Unk1 {  get; set; }
        public uint Unk2 { get; set; }
        public uint FileSize { get; set; }
        public uint Unk3 { get; set; }
        public ushort EntriesCount { get; set; }
        public ushort Unk4 { get; set; }
    }

    public class LSQTREEFile
    {
        public LSQTREEHeader Header { get; set; }
        public List<LSQTREEEntry> Entries { get; set; } = new List<LSQTREEEntry>();
    }
}
