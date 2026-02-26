using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System.Globalization;

namespace RDBExplorer.Core.Formats.ObjectDatabaseFile
{
    public class KidsObjNameTypeIDHelper
    {

        private static readonly Lazy<KidsObjNameTypeIDHelper> _instance = new(() => new KidsObjNameTypeIDHelper());
        public static KidsObjNameTypeIDHelper Instance => _instance.Value;

        // Key = FileKtid, Value = Real Name
        private Dictionary<uint, NameInfo> _knownNames = new();

        private Dictionary<uint, string> _knownProperties = new();
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

        public void LoadProperties (string path)
        {
            // read from csv
            if (!File.Exists(path))
                return;

            _knownProperties.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 2) continue;

                string hexHash = parts[0].Trim().Replace("0x", "");
                if (uint.TryParse(hexHash, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint ktid))
                {
                    _knownProperties[ktid] = parts[1].Trim();
                }
            }
        }

        public string GetPropertyName(uint ktid)
        {
            if (_knownProperties.TryGetValue(ktid, out var name))
            {
                return name;
            }
            return null;
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
