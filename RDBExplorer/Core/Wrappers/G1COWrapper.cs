using RDBExplorer.Core.Formats.G1CO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    internal class G1COWrapper : ResourceWrapper<G1COFile>
    {
        private readonly G1COParser _parser = new G1COParser();

        public override bool IsConvertedToText => true;

        public override List<EntryData> GetEntries()
        {
            return new List<EntryData>();
        }

        public override void Load(byte[] data)
        {
            _parser.Parse(data);
            Model = _parser.GetCOFile;
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
    }
}
