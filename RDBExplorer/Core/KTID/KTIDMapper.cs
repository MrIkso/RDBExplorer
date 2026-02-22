using RDBExplorer.Core.ObjectDatabaseFile;

namespace RDBExplorer.Core.KTID
{
    public class KtidMapResult
    {
        public uint FileIndex { get; set; }
        public uint KtidHash { get; set; }
        public string PhysicalFileHash { get; set; }
        public uint ResourceNameKtid { get; set; }
    }


    public class KTIDMapper
    {
        public List<KtidMapResult> Map(KTIDParser ktidFile, KidsObjDbParser objDb)
        {
            var results = new List<KtidMapResult>();
            var dbLookup = objDb.Objects.ToDictionary(o => o.NameHash, o => o);

            foreach (var ktid in ktidFile.Entries)
            {
                var result = new KtidMapResult
                {
                    FileIndex = ktid.Index,
                    KtidHash = ktid.KtidHash
                };

                if (dbLookup.TryGetValue(ktid.KtidHash, out var dbObject))
                {
                    result.ResourceNameKtid = dbObject.NameHash;
                    if (dbObject.Columns.Count > 0 && dbObject.Columns[0].Values.Count > 0)
                    {
                        var firstVal = dbObject.Columns[0].Values[0];
                        if (firstVal is uint fileHash)
                        {
                            result.PhysicalFileHash = $"0x{fileHash:X8}";
                        }
                    }
                }

                results.Add(result);
            }

            return results;
        }
    }
}
