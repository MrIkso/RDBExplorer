using RDBExplorer.Core.Formats.SID;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    public class SidWrapper : ResourceWrapper<SIDFile>
    {
        private readonly SidParser _parser = new SidParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.SIDFile;
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
