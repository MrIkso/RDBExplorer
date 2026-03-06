using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System.Globalization;
using YamlDotNet.Serialization;

namespace RDBExplorer.Core.Formats.ObjectDatabaseFile
{
    public class KidsObjNameTypeIDHelper
    {
        private static readonly Lazy<KidsObjNameTypeIDHelper> _instance = new(() => new KidsObjNameTypeIDHelper());
        public static KidsObjNameTypeIDHelper Instance => _instance.Value;

        private Dictionary<uint, NameInfo> _knownNames = new();
        private Dictionary<uint, string> _knownProperties = new();

        private KidsObjNameTypeIDHelper() { }

        public void Load(string path)
        {
            if (!File.Exists(path)) 
                return;

            using (var reader = new StreamReader(path))
            {
                var deserializer = new DeserializerBuilder().Build();
                var ymlData = deserializer.Deserialize<KidsObjYml>(reader);

                if (ymlData?.Types != null)
                {
                    _knownNames = ymlData.Types
                        .Where(t => t.Name != null)
                        .GroupBy(t => t.Name.Hash)
                        .ToDictionary(g => g.Key, g => g.First().Name);
                }
            }
        }

        public void LoadProperties(string path)
        {
            if (!File.Exists(path))
                return;
            var newProperties = new Dictionary<uint, string>();

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) 
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length < 2)
                        continue;

                    string hexHash = parts[0].Trim();
                    if (hexHash.StartsWith("0x")) hexHash = hexHash.Substring(2);

                    if (uint.TryParse(hexHash, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint ktid))
                    {
                        newProperties[ktid] = parts[1].Trim();
                    }
                }
            }

            _knownProperties = newProperties;
        }


        public string GetPropertyName(uint ktid) =>
            _knownProperties.TryGetValue(ktid, out var name) ? name : null;

        public string GetFullName(uint ktid) =>
            _knownNames.TryGetValue(ktid, out var nameInfo) ? nameInfo.FullName : null;

        public string GetLocalName(uint ktid)
        {
            if (_knownNames.TryGetValue(ktid, out var nameInfo))
                return nameInfo.LocalName;

            string name = TypeIDHelper.GetTypeName(ktid);
            return !name.StartsWith("Unknown") ? name : null;
        }
    }
}