using RDBExplorer.Core.Formats;
using RDBExplorer.Core.Formats.ObjectDatabaseFile;
using RDBExplorer.Core.Models;
using RDBExplorer.Services;
using RDBExplorer.Utils;
using System.Text;

namespace RDBExplorer.Core
{
    public class ArchiveExploler
    {
        private const int CHUNK_SIZE_DECOMPRESSED = 0x4000; // 16 KB

        public struct WorkerStatus
        {
            public bool IsSuccessed { get; set; }
            public string ErrorMessage { get; set; }

            public WorkerStatus(bool status)
            {
                IsSuccessed = status;
            }

            public WorkerStatus(bool status, string message)
            {
                IsSuccessed = status;
                ErrorMessage = message;
            }
        }

        public List<RDBEntry> RDBEntries { get; private set; }
        private Dictionary<uint, RDBEntry> _ktidCache = new();

        private string _workDir;

        public void Browse(string rdbFilePath)
        {
            if (!File.Exists(rdbFilePath))
            {
                return;
            }
            _workDir = Path.GetDirectoryName(rdbFilePath);
            var rdb = new RDBReader();
            RDBEntries = rdb.Read(rdbFilePath);
            _ktidCache = RDBEntries.GroupBy(e => e.FileKtid).ToDictionary(g => g.Key, g => g.First());
        }

        public byte[]? GetEntryData(RDBEntry entry)
        {
            string countainerPath = Path.Combine(_workDir, entry.Location.ContainerPath);
            if (!File.Exists(countainerPath))
            {
                return null;
                //throw new FileNotFoundException($"Container not found: {countainerPath}");
            }

            using (var fsInput = new FileStream(countainerPath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fsInput))
            {
                long fileOffsetInContainer = 0L;
                if (entry.Location.NewFlags == RDBFlagsNew.Internal)
                {
                    fileOffsetInContainer = (long)entry.Location.Offset;
                }

                fsInput.Position = fileOffsetInContainer;

                KRDIEntry kRDIEntry = ReadKRDIContainer(reader);

                // goto data offset
                //fsInput.Position = fileOffsetInContainer + (long)(alllBlockSize - compressedSize);
                RDBFlags flags = kRDIEntry.Header.Flags;

                uint rawFlags = (uint)kRDIEntry.Header.Flags;

                uint compressionType = (rawFlags >> 20) & 0x3F;

                bool isZlib = (compressionType == (uint)RDBFlags.CompressionZlib);
                bool isZlibExtended = (compressionType == (uint)RDBFlags.CompressionExtended);
                bool isEncrypted = (compressionType == (uint)RDBFlags.CompressionEncrypted);

                long uncompressedSize = kRDIEntry.Header.UncompressedSize;
                if (isZlib || isZlibExtended)
                {
                    // decompress logic
                    byte[] outputBuffer = new byte[uncompressedSize];
                    long currentExtractedSize = 0;

                    while (currentExtractedSize < uncompressedSize)
                    {
                        uint zSize;
                        if (isZlibExtended)
                        {
                            // custom 10 bytes header
                            zSize = reader.ReadUInt16();
                            fsInput.Seek(8, SeekOrigin.Current);
                        }
                        else
                        {
                            zSize = reader.ReadUInt32();
                        }

                        if (zSize == 0 || zSize == 0xFFFFFFFF)
                            break;

                        byte[] compressedChunk = reader.ReadBytes((int)zSize);

                        int remaining = (int)(uncompressedSize - currentExtractedSize);
                        int expectedSize = Math.Min(remaining, CHUNK_SIZE_DECOMPRESSED);

                        byte[] decompressedChunk = CompressUtils.DecompressZlibChunk(compressedChunk, expectedSize);
                        Buffer.BlockCopy(decompressedChunk, 0, outputBuffer, (int)currentExtractedSize, decompressedChunk.Length);
                        currentExtractedSize += decompressedChunk.Length;
                    }
                    return outputBuffer;
                }
                // idk what it, maybe encrypted or just raw data, try read as uncompressed size
                else if (isEncrypted)
                {
                    return reader.ReadBytes((int)kRDIEntry.Header.UncompressedSize);
                }

                // extract data is raw might be uncompressed
                else
                {
                    return reader.ReadBytes((int)uncompressedSize);
                }
            }
        }

        private KRDIEntry ReadKRDIContainer(BinaryReader reader)
        {
            KRDIEntry kRDIEntry = new KRDIEntry();

            KRDIHeader kRDIHeader = new KRDIHeader();
            string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (magic != "IDRK")
            {
                throw new Exception($"Expected IDRK magic, got {magic}");
            }

            kRDIHeader.Magic = magic;
            kRDIHeader.Version = Encoding.ASCII.GetString(reader.ReadBytes(4));
            kRDIHeader.AllBlockSize = reader.ReadInt64();
            kRDIHeader.CompressedSize = reader.ReadInt64();
            kRDIHeader.UncompressedSize = reader.ReadInt64();
            kRDIHeader.ParamDataSize = reader.ReadInt32();
            kRDIHeader.HashName = reader.ReadInt32();
            kRDIHeader.HashType = reader.ReadInt32();
            kRDIHeader.Flags = (RDBFlags)reader.ReadUInt32();
            kRDIHeader.ResourceId = reader.ReadUInt32();
            kRDIHeader.ParamCount = reader.ReadInt32();

            kRDIEntry.Header = kRDIHeader;

            if (kRDIHeader.ParamCount > 0)
            {
                List<KRDIParam> krdiParams = new List<KRDIParam>();
                for (int i = 0; i < kRDIHeader.ParamCount; i++)
                {
                    var param = new KRDIParam();
                    param.Type = reader.ReadInt32();
                    param.Unk = reader.ReadUInt32();
                    param.HashName = reader.ReadInt32();
                    krdiParams.Add(param);
                }

                kRDIEntry.KRDIParams = krdiParams;
                kRDIEntry.ParamData = reader.ReadBytes(kRDIHeader.ParamDataSize);
            }
            return kRDIEntry;
        }

        public WorkerStatus Extract(RDBEntry entry, string outputFolder, bool withName)
        {
            try
            {
                byte[]? data = GetEntryData(entry);

                if (data == null)
                {
                    return new WorkerStatus(false, "Failed to get entry data");
                }

                string fileName = string.Empty;
                if (withName)
                {
                    fileName = entry.Name ?? $"0x{entry.FileKtid:X8}{TypeIDHelper.GetExtension(entry.TypeInfoKtid)}";
                }
                else
                {
                    fileName = $"0x{entry.FileKtid:X8}{TypeIDHelper.GetExtension(entry.TypeInfoKtid)}";
                }
                string outPath = Path.Combine(outputFolder, fileName);
                string directory = Path.GetDirectoryName(outPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(outPath, data);

                Console.WriteLine($"[Done] Extracted: {fileName}");
                return new WorkerStatus(true);
            }
            catch (Exception ex)
            {
                return new WorkerStatus(false, ex.Message);
            }
        }

        public WorkerStatus InjectData(RDBEntry entry, byte[] modData, string rdbFilePath)
        {
            try
            {
                string containerPath = Path.Combine(_workDir, entry.Location.ContainerPath);
                if (!File.Exists(containerPath))
                {
                    return new WorkerStatus(false, "Container not found");
                }

                KRDIEntry originalContainer;

                using (var fsRead = new FileStream(containerPath, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fsRead))
                {
                    fsRead.Position = (long)entry.Location.Offset;
                    originalContainer = ReadKRDIContainer(reader);
                }

                byte[] fullModdedBlock = CreateModifiedIDRK(originalContainer, modData);

                long newOffset;
                using (var fsAppend = new FileStream(containerPath, FileMode.Append, FileAccess.Write))
                {
                    long padding = (16 - (fsAppend.Position % 16)) % 16;
                    for (int i = 0; i < padding; i++)
                    {
                        fsAppend.WriteByte(0);
                    }

                    newOffset = fsAppend.Position;
                    fsAppend.Write(fullModdedBlock, 0, fullModdedBlock.Length);
                }

                entry.Location.Offset = (ulong)newOffset;
                entry.Location.SizeInContainer = (uint)fullModdedBlock.Length;
                entry.FileSize = modData.Length;

                UpdateRDBFile(rdbFilePath, entry);

                Console.WriteLine($"[Success] Injected 0x{entry.FileKtid:X8} with original metadata.");
                // update rdb enrty in gloal list

                return new WorkerStatus(true);
            }
            catch (Exception ex)
            {
                return new WorkerStatus(false, ex.Message);
            }
        }

        private byte[] CreateModifiedIDRK(KRDIEntry original, byte[] modData)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                uint rawFlags = (uint)original.Header.Flags;
                uint noCompressionFlags = rawFlags & ~(0x3Fu << 20);

                int paramsSize = (original.KRDIParams?.Count ?? 0) * 12;
                long totalBlockSize = 56 + paramsSize + original.Header.ParamDataSize + modData.Length;

                writer.Write(Encoding.ASCII.GetBytes("IDRK"));
                writer.Write(Encoding.ASCII.GetBytes(original.Header.Version));
                writer.Write(totalBlockSize);
                writer.Write((long)modData.Length);
                writer.Write((long)modData.Length);
                writer.Write(original.Header.ParamDataSize);
                writer.Write(original.Header.HashName);
                writer.Write(original.Header.HashType);
                writer.Write(noCompressionFlags);
                writer.Write(original.Header.ResourceId);
                writer.Write(original.Header.ParamCount);

                if (original.Header.ParamCount > 0 && original.KRDIParams != null)
                {
                    foreach (var p in original.KRDIParams)
                    {
                        writer.Write(p.Type);
                        writer.Write(p.Unk);
                        writer.Write(p.HashName);
                    }
                    writer.Write(original.ParamData);
                }

                writer.Write(modData);

                return ms.ToArray();
            }
        }

        private void UpdateRDBFile(string rdbPath, RDBEntry entry)
        {
            using (var fsRdb = new FileStream(rdbPath, FileMode.Open, FileAccess.ReadWrite))
            using (var writer = new BinaryWriter(fsRdb))
            {
                // goto file entry position
                long fileSizePos = entry.EntryOffsetInRDB + 24;
                fsRdb.Position = fileSizePos;
                writer.Write((long)entry.FileSize);

                // goto medata location
                int metadataOffset = 48 + (entry.UnkContent?.Length ?? 0);
                fsRdb.Position = entry.EntryOffsetInRDB + metadataOffset;

                // write new flags
                writer.Write((ushort)entry.Location.NewFlags);

                if (entry.DataSize == 0x11) // 64-bit offset format
                {
                    byte high = (byte)((entry.Location.Offset >> 32) & 0xFF);
                    uint low = (uint)(entry.Location.Offset & 0xFFFFFFFF);

                    writer.Write(high);
                    fsRdb.Seek(3, SeekOrigin.Current);
                    writer.Write(low);
                }
                else // 0x0D - 32-bit offset format
                {
                    writer.Write((uint)entry.Location.Offset);
                }
                writer.Write((uint)entry.Location.SizeInContainer);
            }
        }

        public RDBEntry? FindEntryByKtId(uint ktid)
        {
            if (_ktidCache.TryGetValue(ktid, out var entry))
            {
                return entry;
            }
            return null;
        }

        public static string MakeName(RDBEntry entry)
        {
            bool withName = SettingsService.Instance.Config.ExportWithNames;
            string fileName = string.Empty;
            if (withName)
            {
                fileName = entry.Name ?? $"0x{entry.FileKtid:X8}{TypeIDHelper.GetExtension(entry.TypeInfoKtid)}";
            }
            else
            {
                fileName = $"0x{entry.FileKtid:X8}{TypeIDHelper.GetExtension(entry.TypeInfoKtid)}";
            }
            return fileName;
        }
    }
}
