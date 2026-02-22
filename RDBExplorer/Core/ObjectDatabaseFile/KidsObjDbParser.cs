using RDBExplorer.Core.Models;
using RDBExplorer.Utils;

namespace RDBExplorer.Core.ObjectDatabaseFile
{
    public class KidsObjDbParser
    {
        public KidsObjDbHeader Header { get; private set; }
        public List<KidsOdbObject> Objects { get; private set; } = new List<KidsOdbObject>();

        public void Load(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            Load(data);
        }

        public void Load(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                string sig = br.ReadEncodedString(4);
                if (sig != KidsObjDbConstants.KOD_SIGNATURE)
                {
                    throw new Exception("Invalid KOD_ signature");
                }

                Header = new KidsObjDbHeader
                {
                    Version = br.ReadEncodedString(4),
                    HeaderSize = br.ReadUInt32(),
                    Platform = (KoeiPlatform)br.ReadUInt32(),
                    NumEntries = br.ReadUInt32(),
                    NameFile = br.ReadUInt32(),
                    FileSize = br.ReadUInt32()
                };

                Header.Magic = sig;

                br.BaseStream.Position = Header.HeaderSize;

                for (int i = 0; i < Header.NumEntries; i++)
                {
                    long entryStartPos = br.BaseStream.Position;
                    string entrySig = br.ReadEncodedString(4);

                    var obj = new KidsOdbObject();
                    uint numColumns = 0;
                    obj.Magic = entrySig;
                    if (entrySig == KidsObjDbConstants.KODI_SIGNATURE)
                    {
                        obj.IsReference = false;
                        obj.Version = br.ReadEncodedString(4);
                        obj.EntrySize = br.ReadUInt32();
                        obj.NameHash = br.ReadUInt32();
                        obj.TypeKtid = br.ReadUInt32();
                        numColumns = br.ReadUInt32();
                    }
                    else if (entrySig == KidsObjDbConstants.KODR_SIGNATURE)
                    {
                        obj.IsReference = true;
                        obj.Version = br.ReadEncodedString(4);
                        obj.EntrySize = br.ReadUInt32();
                        obj.NameHash = br.ReadUInt32();
                        obj.ParentObjectFileKtid = br.ReadUInt32();
                        obj.ParentObjectKtid = br.ReadUInt32();
                        numColumns = br.ReadUInt32();
                    }
                    else
                    {
                        throw new Exception($"Unknown entry signature 0x{entrySig:X} at 0x{entryStartPos:X}");
                    }

                    for (int c = 0; c < numColumns; c++)
                    {
                        obj.Columns.Add(new KidsOdbColumn
                        {
                            Type = (KidsOdbType)br.ReadInt32(),
                            RowCount = br.ReadUInt32(),
                            NameHash = br.ReadUInt32()
                        });
                    }

                    foreach (var col in obj.Columns)
                    {
                        for (int r = 0; r < col.RowCount; r++)
                        {
                            col.Values.Add(ReadValue(br, col.Type));
                        }
                    }

                    long endPos = entryStartPos + obj.EntrySize;
                    if (br.BaseStream.Position < endPos)
                        br.BaseStream.Position = endPos;

                    long mod = br.BaseStream.Position % 4;
                    if (mod != 0)
                    {
                        br.BaseStream.Position += (4 - mod);
                    }

                    Objects.Add(obj);
                }
            }
        }

        private object ReadValue(BinaryReader br, KidsOdbType type)
        {
            return type switch
            {
                KidsOdbType.Int8 => br.ReadSByte(),
                KidsOdbType.Uint8 => br.ReadByte(),
                KidsOdbType.Int16 => br.ReadInt16(),
                KidsOdbType.Uint16 => br.ReadUInt16(),
                KidsOdbType.Int32 => br.ReadInt32(),
                KidsOdbType.Uint32 => br.ReadUInt32(),
                KidsOdbType.Float => br.ReadSingle(),
                KidsOdbType.Vector2 => new[] { br.ReadSingle(), br.ReadSingle() },
                KidsOdbType.Vector3 => new[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle() },
                KidsOdbType.Vector4 => new[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle() },
                _ => throw new Exception($"Unsupported ODB type: {type}")
            };
        }

    }
}