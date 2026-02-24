using RDBExplorer.Core.Formats.OBORO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    public class OBOROWrapper : ResourceWrapper<OBOROFile>
    {
        private readonly OBOROParser _parser = new OBOROParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.GetOBOROFile;
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
