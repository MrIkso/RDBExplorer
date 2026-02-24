using RDBExplorer.Core.Formats.KTID;
using System.Text.Json;

namespace RDBExplorer.Core.Wrappers
{
    public class KTIDWrapper : ResourceWrapper<List<KTIDEntry>>
    {
        private readonly KTIDParser _parser = new KTIDParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.Entries;
        }

        public override string GetJsonData() =>
            JsonSerializer.Serialize(Model, new JsonSerializerOptions { WriteIndented = true });

        public override List<EntryData> GetEntries() => new();

        public override bool IsConvertedToText => true;
        
    }
}
