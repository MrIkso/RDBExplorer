using RDBExplorer.Core.Formats.KTS;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    public class KTSWrapper : ResourceWrapper<KTSFile>
    {
        private readonly KTSParser _parser = new KTSParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.KTSFile;
        }

        public override async Task SerializeJsonToStreamAsync(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };

            await JsonSerializer.SerializeAsync(stream, Model, options);
        }

        public override List<EntryData> GetEntries() => new();

        public override bool IsConvertedToText => true;

    }
}
