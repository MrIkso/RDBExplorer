using RDBExplorer.Core.Formats.ObjectDatabaseFile;
using RDBExplorer.Utils.JsonConverters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    public class KidsObjDbParserWrapper : ResourceWrapper<KidsOdbObjectFile>
    {
        private readonly KidsObjDbParser _parser = new KidsObjDbParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.KidsOdbObjectFile;
        }

        public override async Task SerializeJsonToStreamAsync(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new JsonVectorConverters());
            options.Converters.Add(new JsonVector3Converter());
            options.Converters.Add(new JsonVector4Converter());
            await JsonSerializer.SerializeAsync(stream, Model, options);
        }

        public override List<EntryData> GetEntries() => new();

        public override bool IsConvertedToText => true;

    }
}
