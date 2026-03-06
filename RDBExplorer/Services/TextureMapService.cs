using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System.Text.Json;

namespace RDBExplorer.Services
{
    public class TextureMapService
    {
        private static TextureMapService _instance;
        private static readonly object _lock = new object();

        public static TextureMapService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        throw new Exception("TextureMapService must be initialized first!");
                    return _instance;
                }
            }
        }

        public Dictionary<uint, uint[]> ModelToTextures { get; private set; }

        private TextureMapService(string path)
        {
            if (!File.Exists(path)) 
                return;

            using (FileStream fs = File.OpenRead(path))
            {
                var rootData = JsonSerializer.Deserialize<G1M2G1TModel>(fs);

                if (rootData?.Mappings != null)
                {
                    ModelToTextures = rootData.Mappings.ToDictionary(
                        mapping => CommonUtils.ParseHex(mapping.G1mHash),
                        mapping => mapping.G1tHashes
                            .OrderBy(kvp => int.Parse(kvp.Key))
                            .Select(kvp => CommonUtils.ParseHex(kvp.Value))
                            .ToArray()
                    );
                }
            }
        }

        public static void Initialize(string path)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new TextureMapService(path);
                }
            }
        }
    }
}
