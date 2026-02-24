using RDBExplorer.Utils;
using RDBExplorer.Utils.JsonConverters;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.OIDSQ
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OIDSQEntry
    {
        public uint Index { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidHash { get; set; }
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public uint Unk5 { get; set; }
        public uint Unk6 { get; set; }
        public uint Unk7 { get; set; }
        public uint Unk8 { get; set; }
        public uint Unk9 { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidObjectHash1 { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidObjectHash2 { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidObjectHash3 { get; set; }
    }

    public class OIDSQFile
    {
        public uint Count { get; set; }
        public List<OIDSQEntry> Entries { get; set; } = new List<OIDSQEntry>();
    }

    public class OIDSQParser
    {
        public OIDSQFile OIDSQFile { get; set; }

        public void Load(byte[] data)
        {
            OIDSQFile = new OIDSQFile();
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                if (data.Length < 8)
                    return;

                OIDSQFile.Count = br.ReadUInt32();

                for (int i = 0; i < OIDSQFile.Count + 1; i++)
                {
                    var entry = br.ReadStruct<OIDSQEntry>();
                    OIDSQFile.Entries.Add(entry);
                }
            }
        }
    }
}

