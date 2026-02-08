using RDBExplorer.Core.Models;
using System.Text;

namespace RDBExplorer.Core
{
    public class RDBReader
    {
        private const int RDB_ENTRY_HEADER_SIZE = 48;
        private RDBHeader _RDBHeader;
        private List<RdxEntry> rdxEntries;

        public List<RDBEntry> Read(string filePath)
        {
            string workDir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string rdxFile = Path.Combine(workDir, string.Format("{0}.rdx", fileName));
            Console.WriteLine(rdxFile);

            RDXReader rDXReader = new RDXReader(rdxFile);
            rdxEntries = rDXReader.RdxEntries;

            var entries = new List<RDBEntry>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                var headerMagic = Encoding.ASCII.GetString(reader.ReadBytes(4));
                if (headerMagic != "_DRK")
                    throw new Exception("Not a valid RDB file");

                _RDBHeader = new RDBHeader
                {
                    Magic = headerMagic,
                    Version = reader.ReadInt32(),
                    HeaderSize = reader.ReadInt32(),
                    SystemId = reader.ReadInt32(),
                    FileCount = reader.ReadInt32(),
                    DatabaseId = reader.ReadUInt32(),
                    FolderPath = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd('\0')
                };

                for (int i = 0; i < _RDBHeader.FileCount; i++)
                {
                    // allign by 4 byte
                    while (fs.Position % 4 != 0)
                    {
                        fs.ReadByte();
                    }

                    long entryStartPos = fs.Position;
                    // Console.WriteLine($"RDBEntry start pos 0x{entryStartPos:X}");
                    var entry = new RDBEntry
                    {
                        EntryOffsetInRDB = entryStartPos,
                        Magic = Encoding.ASCII.GetString(reader.ReadBytes(4)),
                        Version = reader.ReadUInt32(),
                        EntrySize = reader.ReadInt64(),
                        DataSize = reader.ReadInt64(), // c_size
                        FileSize = reader.ReadInt64(),
                        EntryType = reader.ReadUInt32(),
                        FileKtid = reader.ReadUInt32(),
                        TypeInfoKtid = reader.ReadUInt32(),
                        Flags = (RDBFlags)reader.ReadUInt32()
                    };

                    int metadataSize = (int)entry.DataSize;
                    
                    int allParamsSize = (int)entry.EntrySize - metadataSize - RDB_ENTRY_HEADER_SIZE;

                    if (allParamsSize > 0)
                    {
                        entry.UnkContent = reader.ReadBytes(allParamsSize);
                    }

                    ParseLocation(reader, entry, metadataSize);

                    entries.Add(entry);
                }
            }
            return entries;
        }

        private void ParseLocation(BinaryReader reader, RDBEntry entry, int metadataSize)
        {
            if (metadataSize == 0)
            {
                Console.WriteLine("skip parse location");
                return;
            }
            if (metadataSize == 0x11) // calculate 64 bit data offset
            {
                entry.Location.NewFlags = (RDBFlagsNew)reader.ReadUInt16();

                byte highByte = reader.ReadByte();
                reader.ReadBytes(3); //skip
                uint lowBytes = reader.ReadUInt32();

                entry.Location.Offset = ((ulong)highByte << 32) | lowBytes;
                entry.Location.SizeInContainer = reader.ReadUInt32();
                entry.Location.FDataId = reader.ReadUInt16();
                reader.ReadByte();
            }

            if (metadataSize == 0x0D) // calcualte 32 bit data offset
            {
                entry.Location.NewFlags = (RDBFlagsNew)reader.ReadUInt16();

                entry.Location.Offset = reader.ReadUInt32();
                entry.Location.SizeInContainer = reader.ReadUInt32();
                entry.Location.FDataId = reader.ReadUInt16();
                reader.ReadByte(); // Padding
            }
            // make location to container
            ResolveContainerPath(entry);
        }

        private void ResolveContainerPath(RDBEntry entry)
        {
            string hexId = entry.FileKtid.ToString("X8").ToLower();
            if (entry.Location.NewFlags == RDBFlagsNew.External)
            {
                uint hash = entry.FileKtid;
                string folderPrefix = (hash & 0xFF).ToString("X2").ToLower();
                string relativePath = $"{_RDBHeader.FolderPath}{folderPrefix}/0x{hexId}.file";
                entry.Location.ContainerPath = relativePath;
               /* Console.WriteLine($"hexId {hexId} at {relativePath}");*/
            }
            else
            {
                int idxIndex = entry.Location.FDataId;
                var rdxMatch = rdxEntries.FirstOrDefault(x => x.Index == idxIndex);
                string containerHash = rdxMatch.FileId.ToString("X8").ToLower();
                string containerName = $"0x{containerHash}.fdata";
                entry.Location.ContainerPath = containerName;
            }
        }
    }
}
