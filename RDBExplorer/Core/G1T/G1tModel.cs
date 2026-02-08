namespace RDBExplorer.Core.G1T
{
    public enum G1tPlatform : uint
    {
        Generic = 0,
        PC = 10,
        Console = 14
    }

    public class G1tHeader
    {
        public string Magic { get; set; } // "G1TG"
        public string Version { get; set; } // "0060"
        public uint FileSize { get; set; }
        public uint TableOffset { get; set; }
        public uint NumTextures { get; set; }
        public G1tPlatform Platform { get; set; }
        public uint UnkDataSize { get; set; }
        public uint Unk1C { get; set; }
    }

    public class G1tMipMap
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public byte[] Data { get; set; }
    }

    public class G1tTexture
    {
        public int MipCount { get; set; }
        public byte Sys { get; set; }
        public byte Format { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public byte[] Unk3 { get; set; }
        public byte ExtraHeaderVersion { get; set; }

        public uint ArraySize { get; set; }
        public byte[] ExtraHeaderRaw { get; set; }
        public List<G1tMipMap> MipMaps { get; set; } = new List<G1tMipMap>();

        public string GetFormatName() => G1tFormats.GetDxgiName(Format);
    }
}
