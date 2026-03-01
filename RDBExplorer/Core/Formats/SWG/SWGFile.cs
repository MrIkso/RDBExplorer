namespace RDBExplorer.Core.Formats.SWG
{
    public struct SWGEntry
    {
        public ushort Id1 { get; set; }
        public ushort Id2 { get; set; }
        public uint Stiffness { get; set; }
        public float Damping { get; set; }
        public float Gravity { get; set; }
        public float Inertia { get; set; }
        public float Friction { get; set; }
        public float WindRate { get; set; }
        public float LimitAngle { get; set; }
        public float SpringRate { get; set; }
        public float Noise { get; set; }
        public float NoiseRate { get; set; }
        public float Drag { get; set; }
        public byte ColilisonType { get; set; }
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
        public float Margin { get; set; }
        public float Radius { get; set; }
        public float ReactionSpeed { get; set; }
        public byte Unk4 { get; set; }
        public byte Unk5 { get; set; }
        public byte Unk6 { get; set; }
        public byte Unk7 { get; set; }
        public byte Unk8 { get; set; }
        public byte Unk9 { get; set; }
        public byte Unk10 { get; set; }
        public byte Unk11 { get; set; }
    }

    public class SWGHeader
    {
        public string Magic {  get; set; }
        public uint BlockSize { get; set; }
        public uint EntryCount { get; set; }
        public string GroupMame { get; set; }
        public float GlobalScale { get; set; }
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
    }
    public class SWGFile
    {
        public SWGHeader Header { get; set; }
        public List<SWGEntry> Entries { get; set; } = new List<SWGEntry>();
    }
}
