using RDBExplorer.Core.Formats.G1MX;

namespace RDBExplorer.Core.Wrappers
{
    public class G1MXWrapper : ResourceWrapper<G1MXFile>
    {
        private readonly G1MXFileParser _parser = new G1MXFileParser();

        public override void Load(byte[] data)
        {
            Model = _parser.Parse(data);
        }

        public override List<EntryData> GetEntries()
        {
            var result = new List<EntryData>();
            var models = Model?.G1MX?.G1MXF?.GMXM?.G1M_ModelsList;

            if (models == null) return result;

            for (int i = 0; i < models.Count; i++)
            {
                result.Add(new EntryData
                {
                    Name = $"Model_{i}_{models[i].Magic}.g1m",
                    Data = models[i].Data
                });
            }
            return result;
        }

        public override bool IsConvertedToText => false;
    }

}
