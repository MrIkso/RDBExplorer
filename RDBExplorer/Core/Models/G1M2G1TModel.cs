using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Models
{
    public class Summary
    {
        [JsonPropertyName("g1m_count")]
        public int G1mCount { get; set; }
    }

    public class Mapping
    {
        [JsonPropertyName("g1m_hash")]
        public string G1mHash { get; set; }

        [JsonPropertyName("ktid_hashes")]
        public string KtidHashes { get; set; }

        [JsonPropertyName("g1t_hashes")]
        public Dictionary<string, string> G1tHashes { get; set; }

        [JsonPropertyName("kidsobjdb_count")]
        public int KidsobjdbCount { get; set; }

        [JsonPropertyName("kidsobjdb_files")]
        public string KidsobjdbFiles { get; set; }
    }

    public class G1M2G1TModel
    {
        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }

        [JsonPropertyName("mappings")]
        public List<Mapping> Mappings { get; set; }
    }
}
