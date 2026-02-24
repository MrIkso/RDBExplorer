using RDBExplorer.Utils;
using RDBExplorer.Utils.JsonConverters;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.OBORO
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OBOROEntry
    {
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidHash1 { get; set; }

        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidHash2 { get; set; }

        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidHash3 { get; set; }
    }

    public class OBOROFile
    {
        public uint Count { get; set; }
        public List<OBOROEntry> EntryList { get; set; } = new List<OBOROEntry>();

        public List<byte> Types { get; set; } = new List<byte>() { };

    }

    public class OBOROParser
    {
        public OBOROFile GetOBOROFile { get; set; }

        public void Load(byte[] data)
        {
            GetOBOROFile = new OBOROFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms, Encoding.ASCII))
            {

                uint count = br.ReadUInt32();

                GetOBOROFile.Count = count;

                for (int i = 0; i < count; i++)
                {
                    OBOROEntry entry = br.ReadStruct<OBOROEntry>();
                    GetOBOROFile.EntryList.Add(entry);
                }

                for (int i = 0; i < count; i++)
                {
                    byte type = br.ReadByte();
                    GetOBOROFile.Types.Add(type);
                }
            }
        }
    }
}
