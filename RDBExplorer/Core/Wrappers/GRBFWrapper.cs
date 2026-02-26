using RDBExplorer.Core.Formats.GRBF;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    public class GRBFWrapper : ResourceWrapper<GRBFFile>
    {
        private readonly GRBFParser _parser = new GRBFParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.File;
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
