namespace RDBExplorer.Core.Formats.G1MX
{
    public class KG1M
    {
        public string Magic { get; set; }
        public uint Version { get; set; }
        public uint HeaderSize { get; set; }
        public uint TextLen { get; set; }
        public string Text { get; set; }
    }

    public class GResourceChunk
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint DataSize { get; set; }
        public byte[] Data { get; set; }
    }

    public class G1M
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint DataSize { get; set; }
        public uint HeaderSize { get; set; }
        public uint Unk { get; set; }
        public uint ChunkCount { get; set; }
        public byte[] Data { get; set; }
        public List<GResourceChunk> Chunks = new List<GResourceChunk>();
    }

    public class GMXM
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint DataSize { get; set; }
        public uint ElementCount { get; set; }
        public uint HeaderSize { get; set; }
        public uint[] DependencyList { get; set; }
        public List<G1M> G1M_ModelsList = new List<G1M>();
    }

    public class G1MXF
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint HeaderSize { get; set; }
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Zero { get; set; }
        public GMXM GMXM { get; set; }
    }

    public class G1MX
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint BlockSize { get; set; }
        public uint DataStartPointer { get; set; }
        public G1MXF G1MXF { get; set; }
    }

    public class G1MXFile
    {
        public KG1M KG1M { get; set; }
        public G1MX G1MX {  get; set; }
    }

}
