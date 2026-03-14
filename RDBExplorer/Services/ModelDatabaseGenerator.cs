using RDBExplorer.Core;
using RDBExplorer.Core.Formats.KTID;
using RDBExplorer.Core.Formats.ObjectDatabaseFile;
using RDBExplorer.Core.Models;
using RDBExplorer.Models;
using System.Text.Json;

namespace RDBExplorer.Services
{
    public class ModelDatabaseGenerator
    {
        private readonly ArchiveExploler _explorer;

        public ModelDatabaseGenerator(ArchiveExploler explorer)
        {
            _explorer = explorer;
        }

        public async Task GenerateAndSaveJson(string outputPath, IProgress<GeneratorProgress> progress)
        {
            var mappings = await Task.Run(() => BuildMappings(progress));

            progress.Report(new GeneratorProgress { Status = "Saving JSON file..." });

            var result = new G1M2G1TModel
            {
                Summary = new Summary { G1mCount = mappings.Count },
                Mappings = mappings
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            using (FileStream createStream = File.Create(outputPath))
            {
                await JsonSerializer.SerializeAsync(createStream, result, options);
            }

            progress.Report(new GeneratorProgress { Status = "Done!" });
        }

        private List<Mapping> BuildMappings(IProgress<GeneratorProgress> progress)
        {
            var modelMap = new Dictionary<uint, Mapping>();
            var objDbEntries = _explorer.RDBEntries
                .Where(e => (KTFileType)e.TypeInfoKtid == KTFileType.ObjectDatabaseFile)
                .ToList();

            int total = objDbEntries.Count;
            for (int i = 0; i < total; i++)
            {
                var dbEntry = objDbEntries[i];
                progress.Report(new GeneratorProgress
                {
                    Current = i + 1,
                    Status = $"Processing: {dbEntry.Name} ({i + 1}/{total})"
                });

                byte[]? data = _explorer.GetEntryData(dbEntry);
                if (data == null)
                {
                    
                    continue;
                }

                var parser = new KidsObjDbParser();
                parser.Load(data);

                var objectsById = parser.KidsOdbObjectFile.Objects
                    .GroupBy(o => o.KTID)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var obj in parser.KidsOdbObjectFile.Objects)
                {
                    if (string.IsNullOrEmpty(obj.TypeName) || !obj.TypeName.StartsWith("Model"))
                        continue;

                    ProcessModelObject(obj, objectsById, dbEntry.Name, modelMap);
                }

                
            }

            return modelMap.Values.ToList();
        }

        private void ProcessModelObject(KidsOdbObject modelObj, Dictionary<uint, KidsOdbObject> objectsById, string dbName, Dictionary<uint, Mapping> map)
        {
            uint modelHash = ExtractHash(modelObj, "KTGLModelDataResourceHash");
            uint texturesTableHash = ExtractHash(modelObj, "TexturesRenderStateObjectHash");

            if (modelHash == 0)
            {
                return;
            }

            if (!map.TryGetValue(modelHash, out var mapping))
            {
                mapping = new Mapping
                {
                    G1mHash = $"0x{modelHash:X8}",
                    G1tHashes = new Dictionary<string, string>(),
                    KidsobjdbFiles = dbName
                };
                map[modelHash] = mapping;
            }

            if (texturesTableHash != 0)
            {
                ProcessTexturesTableObject(objectsById, texturesTableHash, mapping);
            }
        }

        private void ProcessTexturesTableObject(Dictionary<uint, KidsOdbObject> objectsById, uint tableHashName, Mapping mapping)
        {
            if (objectsById.TryGetValue(tableHashName, out var textureTableObject))
            {
                uint ktidHash = ExtractHash(textureTableObject, "TextureBindTableCSVFileResourceHash");
                if (ktidHash != 0)
                {
                    mapping.KtidHashes = $"0x{ktidHash:X8}";

                    var textures = ResolveTexturesFromTable(ktidHash, objectsById);
                    int idx = mapping.G1tHashes.Count;
                    foreach (var texHash in textures)
                    {
                        string val = $"0x{texHash:X8}";
                        if (!mapping.G1tHashes.ContainsValue(val))
                        {
                            mapping.G1tHashes[idx.ToString()] = val;
                            idx++;
                        }
                    }
                }
            }
        }

        private List<uint> ResolveTexturesFromTable(uint tableHash, Dictionary<uint, KidsOdbObject> objectsById)
        {
            var textures = new List<uint>();
            var tableEntry = _explorer.FindEntryByKtId(tableHash);
            if (tableEntry == null)
            {
                return textures;
            }

            byte[]? data = _explorer.GetEntryData(tableEntry);
            if (data == null)
            {
                return textures;
            }
            var ktidParser = new KTIDParser();
            ktidParser.Load(data);

            foreach (var entry in ktidParser.Entries)
            {
                if (objectsById.TryGetValue(entry.KtidHash, out var depObj))
                {
                    uint texHash = ExtractHash(depObj, "KTGLTexContextResourceHash");
                    if (texHash == 0)
                    {
                        texHash = ExtractHash(depObj, "LayoutTexturesRenderStateObjectHash");
                    }

                    if (texHash != 0)
                    {
                        textures.Add(texHash);
                    }
                }
            }

            return textures;
        }

        private uint ExtractHash(KidsOdbObject obj, string propertyName)
        {
            var col = obj.Columns.FirstOrDefault(c =>
                c.PropertyName != null && c.PropertyName.Contains(propertyName, StringComparison.OrdinalIgnoreCase));

            if (col != null && col.Values.Count > 0 && col.Values[0] is uint val)
            {
                return val;
            }
            return 0;
        }
    }
}