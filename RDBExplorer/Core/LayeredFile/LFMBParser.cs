using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System.Text;
using System.Text.Json;

namespace RDBExplorer.Core.LayeredFile
{
    public class LFMBParser
    {
        public struct LFMArchiveHeader
        {
            public uint Magic; // LFMB
            public uint Version; // 0
            public long ArchiveCount;
            public long Size;
            public long Align; // 2048 always
        }

        public struct LFMArchiveEntry
        {
            public long Offset;
            public long UncompressedSize;
            public long Size;
            public long IsCompressed;

            public string Name;
        }

        public void UnpackBinArchive(string filePath, string outputFolder)
        {
            string fileName = Path.GetFileName(filePath);
            string rootDir = Path.GetDirectoryName(filePath);
            string[] data = Path.GetFileNameWithoutExtension(filePath).Split("_");

            string lfmOrderPath = Path.Combine(rootDir, $"lfm_order_{data[1].ToString().PadLeft(2, '0')}.bin");
            string lfmInfoPath = Path.Combine(rootDir, $"load_inf_{data[1].ToString().PadLeft(2, '0')}.bin");

            // uused for getting names for arhive enty
            // contains hash
            // not sure for editing
            LFMOrderReader lFMOrderReader = new LFMOrderReader();
            lFMOrderReader.Read(lfmOrderPath);

            // used for getting info about entry if compressed and size
            // need update after update bin archive
            LFMInfFileParser lFMInfFileParser = new LFMInfFileParser();
            lFMInfFileParser.Read(lfmInfoPath);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))

            using (var reader = new BinaryReader(fs))
            {
                var header = new LFMArchiveHeader();
                header.Magic = reader.ReadUInt32();
                header.Version = reader.ReadUInt32();
                header.ArchiveCount = reader.ReadInt64();
                header.Size = reader.ReadInt64();
                header.Align = reader.ReadInt64();

                List<LFMArchiveEntry> entries = new List<LFMArchiveEntry>();
                for (int i = 0; i < header.ArchiveCount; i++)
                {
                    long offset = reader.ReadInt64();
                    long decompSize = reader.ReadInt64();
                    long bolockSize = reader.ReadInt64();
                    long isCompressed = reader.ReadInt64();
                    LFMArchiveEntry entry = new LFMArchiveEntry();
                    entry.Offset = offset;
                    entry.UncompressedSize = decompSize;
                    entry.Size = bolockSize;
                    entry.IsCompressed = isCompressed;
                    entries.Add(entry);
                }

                ArchiveManifest manifest = new ArchiveManifest
                {
                    ArchiveName = fileName,
                    AlignSize = (int)header.Align,
                    Files = new List<ArchiveBinFile>()
                };


                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    reader.BaseStream.Position = entry.Offset;
                    byte[] finalData = null;
                    if (entry.IsCompressed == 1)
                    {
                        byte[] rawData = reader.ReadBytes((int)entry.Size);
                        finalData = CompressUtils.DecompressAssetsBin(rawData);
                    }
                    else
                    {
                        finalData = reader.ReadBytes((int)entry.Size);
                    }
                    // detect ext by header
                    string entryName = $"{Path.GetFileNameWithoutExtension(filePath)}_entry_{i}{FileTypeDetector.DetectExtension(finalData)}";
                    string nameFromArchive = lFMOrderReader.HashedPathNames[i];
                    Console.WriteLine($"Unpacking: {nameFromArchive} -> {entryName}");

                    manifest.Files.Add(new ArchiveBinFile
                    {
                        Entry = entryName,
                        IsCompresed = entry.IsCompressed == 1
                    });

                    /*entry.Name.TrimStart('/', '\\');*/
                    string savePath = Path.Combine(outputFolder, entryName);
                    string saveDir = Path.GetDirectoryName(savePath);
                    if (!string.IsNullOrEmpty(saveDir) && !Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }

                    File.WriteAllBytes(savePath, finalData);
                }

                string jsonPath = Path.Combine(outputFolder, "manifest.json");
                var options = new JsonSerializerOptions 
                {
                    WriteIndented = true,
                };
                string jsonString = JsonSerializer.Serialize(manifest, options);
                File.WriteAllText(jsonPath, jsonString);

            }
        }

      
        public void PackBinArchive(string manifestPath, string outputBinPath)
        {
            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException("manifest.json not found!");
            }
            string jsonContent = File.ReadAllText(manifestPath);
            ArchiveManifest manifest = JsonSerializer.Deserialize<ArchiveManifest>(jsonContent);

            if (manifest == null || manifest.Files == null)
            {
                throw new Exception("Invalid manifest format.");
            }

            long alignment = manifest.AlignSize;
            
            string inputFolder = Path.GetDirectoryName(manifestPath);
            string saveArchivePath = Path.Combine(outputBinPath, manifest.ArchiveName);
           
            List<LFMArchiveEntry> entries = new List<LFMArchiveEntry>();

            using (var fs = new FileStream(saveArchivePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                // write placeholder bytes
                long tableSize = 32 + (manifest.Files.Count * 32);
                writer.Write(new byte[tableSize]);

                foreach (var fileEntry in manifest.Files)
                {
                    string fullPath = Path.Combine(inputFolder, fileEntry.Entry);
                    if (!File.Exists(fullPath))
                        continue;

                    byte[] originalData = File.ReadAllBytes(fullPath);
                    long uncompressedSize = originalData.Length;
                    byte[] dataToWrite;

                    if (/*fileEntry.IsCompresed*/ false)
                        dataToWrite = CompressUtils.CompressAssetsBin(originalData);
                    else
                        dataToWrite = originalData;

                    // aligning
                    long currentPos = writer.BaseStream.Position;
                    long paddingSize = (alignment - (currentPos % alignment)) % alignment;
                    if (paddingSize > 0)
                    {
                        writer.Write(new byte[paddingSize]);
                    }

                    LFMArchiveEntry entry = new LFMArchiveEntry
                    {
                        Offset = writer.BaseStream.Position,
                        UncompressedSize = uncompressedSize,
                        Size = dataToWrite.Length,
                        IsCompressed = /*fileEntry.IsCompresed ? 1 :*/ 0
                    };

                    writer.Write(dataToWrite);
                    
                    entries.Add(entry);
                }

                // alaign end file
                long finalPos = writer.BaseStream.Position;
                long finalPadding = (alignment - (finalPos % alignment)) % alignment;
                if (finalPadding > 0)
                {
                    writer.Write(new byte[finalPadding]);
                }

                // write header
                writer.BaseStream.Position = 0;
                writer.Write(Encoding.ASCII.GetBytes("LFMB"));
                writer.Write(0u);
                writer.Write((long)entries.Count);
                writer.Write(writer.BaseStream.Length);
                writer.Write(alignment);

                // write offsets table
                foreach (var entry in entries)
                {
                    writer.Write(entry.Offset);
                    writer.Write(entry.UncompressedSize);
                    writer.Write(entry.Size);
                    writer.Write(entry.IsCompressed);
                }

                Console.WriteLine($"Created bin archive: {saveArchivePath}");

                CreateLoadInfo(manifest.ArchiveName, outputBinPath, entries);
            }
        }

        private void CreateLoadInfo(string archiveName, string rootDir, List<LFMArchiveEntry> entries)
        {
            archiveName = Path.GetFileNameWithoutExtension(archiveName);
            //string rootDir = Path.GetDirectoryName(binPath);
            //string fileName = Path.GetFileNameWithoutExtension(binPath);
            string[] data = archiveName.Split("_");
            string suffix = data.Length > 1 ? data[1].PadLeft(2, '0') : "00";
            string lfmInfoPath = Path.Combine(rootDir, $"load_inf_{suffix}.bin");

            var lFMInfEntries = new List<LFMInfEntry>();
            for (int i = 0; i < entries.Count; i++)
            {
                lFMInfEntries.Add(new LFMInfEntry
                {
                    Index = i,
                    UnCompressedSize = (int)entries[i].UncompressedSize,
                    IsCompressed = (int)entries[i].IsCompressed
                });
            }

            LFMInfFileParser parser = new LFMInfFileParser();
            parser.Write(lFMInfEntries, lfmInfoPath);
            Console.WriteLine($"Created ifo file: {lfmInfoPath}");
        }
    }

}
