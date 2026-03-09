using RDBExplorer.Core.Formats.KSCL;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    internal class KSCLWrapper : ResourceWrapper<KSCLFile>
    {
        private readonly KSCLParser _parser = new KSCLParser();

        public override bool IsConvertedToText => true;

        public override List<EntryData> GetEntries()
        {
            return new List<EntryData>();
        }

        public override void Load(byte[] data)
        {
            _parser.Parse(data);
            Model = _parser.GetKSCLFile;
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

