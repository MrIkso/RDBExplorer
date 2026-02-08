using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Models
{
    public class ArchiveBinFile
    {
        [JsonPropertyName("entry")]
        public string Entry { get; set; }

        [JsonPropertyName("isCompresed")]
        public bool IsCompresed { get; set; }
    }

    public class ArchiveManifest
    {
        [JsonPropertyName("archive_name")]
        public string ArchiveName { get; set; }

        [JsonPropertyName("align_size")]
        public int AlignSize { get; set; }

        [JsonPropertyName("files")]
        public List<ArchiveBinFile> Files { get; set; }
    }

}
