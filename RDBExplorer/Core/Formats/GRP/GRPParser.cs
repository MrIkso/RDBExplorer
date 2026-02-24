using RDBExplorer.Core.Formats.KTID;
using RDBExplorer.Utils;
using RDBExplorer.Utils.JsonConverters;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.GRP
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GRPEnrty
    {
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidHash { get; set; }
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public uint Unk5 { get; set; }
        public uint Unk6 { get; set; }
        public uint Unk7 { get; set; }
    }

    public class GRPParser
    {
        public List<GRPEnrty> Entries { get; set; } = new List<GRPEnrty>();

        public void Load(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                Entries.Clear();
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    GRPEnrty enrty = br.ReadStruct<GRPEnrty>();
                    Entries.Add(enrty);
                }
            }
        }
    }
}
