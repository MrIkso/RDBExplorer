using RDBExplorer.Core.Formats.MTL;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    public class MTLWrapper : ResourceWrapper<MTLFile>
    {
        private readonly MTLParser _parser = new MTLParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.MTLFile;
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