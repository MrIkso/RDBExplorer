using RDBExplorer.Core.Models;
using RDBExplorer.Utils;

namespace RDBExplorer.Core.ObjectDatabaseFile
{
    public class KidsObjNameTypeIDHelper
    {

        private static readonly Lazy<KidsObjNameTypeIDHelper> _instance = new(() => new KidsObjNameTypeIDHelper());
        public static KidsObjNameTypeIDHelper Instance => _instance.Value;

        // Key = FileKtid, Value = Real Name
        private Dictionary<uint, NameInfo> _knownNames = new();

        private KidsObjNameTypeIDHelper() { }

        public void Load(string path)
        {
            string yamlContent = File.ReadAllText(path);
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            var ymlData = deserializer.Deserialize<KidsObjYml>(yamlContent);
            Dictionary<uint, NameInfo> dictionary = ymlData.Types.Where(t => t.Name != null)
                .GroupBy(t => t.Name.Hash)
                .ToDictionary(g => g.Key, g => g.First().Name);
            _knownNames = dictionary;
        }


        public string GetFullName(uint ktid)
        {
            if (_knownNames.TryGetValue(ktid, out var nameInfo))
            {
                return nameInfo.FullName;
            }
            return null;
        }

        public string GetLocalName(uint ktid)
        {
            if (_knownNames.TryGetValue(ktid, out var nameInfo))
            {
                return nameInfo.LocalName;
            }
            else {

                string name = TypeIDHelper.GetTypeName(ktid);
                if (!name.StartsWith("Unknown"))
                {
                    return name;
                }
            }
            return null;
        }

    }
}
