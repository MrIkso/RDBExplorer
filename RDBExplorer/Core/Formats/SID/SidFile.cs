using RDBExplorer.Utils.JsonConverters;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.SID
{
    public struct SIDEntry
    {
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KTId1 { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KTId2 { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KTId3 { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KTId4 { get; set; }
    }

    public struct SIDHeader {
        public uint Zero {  get; set; }
        public uint Version { get; set; }
        public ushort EntriesCount { get; set; }
    }

    public class SIDFile
    {
        public SIDHeader Header { get; set; }
        public List<SIDEntry> Entries { get; set; } = new List<SIDEntry>();
    }
}
