using RDBExplorer.Core.Formats.TexInfo;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    internal class TextInfoWrapper : ResourceWrapper<TexInfoFile>
    {
        private readonly TextInfoParser _parser = new TextInfoParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.TexInfoFile;
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
