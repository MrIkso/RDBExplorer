using RDBExplorer.Utils.JsonConverters;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.MTL
{
    public struct PhysicsEntry
    {
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint G1MId { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint XSIId { get; set; }
    };

    public class MaterialEntry
    {
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint NameKTID { get; set; }
        public uint IdCount { get; set; }
        public uint[] Ids { get; set; }
    };

    public struct MTLHeader
    {
        public uint NumNames { get; set; }
        public uint NumMaterials { get; set; }
        public uint NumCloths { get; set; }
        public uint NumPonyTails { get; set; }
    }

    public class MTLFile
    {
        public MTLHeader Header { get; set; }

        public List<MaterialEntry> MaterialEntries { get; set; }
        public List<PhysicsEntry> ClothsEntries { get; set; }
        public List<PhysicsEntry> PonyTailsEntries { get; set; }
    }
}
