using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System.Numerics;

namespace RDBExplorer.Core.Formats.ObjectDatabaseFile
{
    public class KidsObjDbParser
    {
        public KidsOdbObjectFile KidsOdbObjectFile { get; set; } = new KidsOdbObjectFile();
        
        public void Load(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            Load(data);
        }

        public void Load(byte[] data)
        {
            List<KidsOdbObject> objects = new List<KidsOdbObject>();
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                string sig = br.ReadEncodedString(4);
                if (sig != KidsObjDbConstants.KOD_SIGNATURE)
                {
                    throw new Exception("Invalid KOD_ signature");
                }

                KidsObjDbHeader header = new KidsObjDbHeader
                {
                    Version = br.ReadEncodedString(4),
                    HeaderSize = br.ReadUInt32(),
                    Platform = (KoeiPlatform)br.ReadUInt32(),
                    NumEntries = br.ReadUInt32(),
                    NameKTID = br.ReadUInt32(),
                    TotalSize = br.ReadUInt32()
                };

                header.Magic = sig;

                br.BaseStream.Position = header.HeaderSize;

                for (int i = 0; i < header.NumEntries; i++)
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
                        obj.KTID = br.ReadUInt32();
                        obj.TypeInfoKTID = br.ReadUInt32();
                        obj.ParentObjectFileKtid = null;
                        obj.ParentObjectKtid = null;
                        numColumns = br.ReadUInt32();
                    }
                    else if (entrySig == KidsObjDbConstants.KODR_SIGNATURE)
                    {
                        obj.IsReference = true;
                        obj.Version = br.ReadEncodedString(4);
                        obj.EntrySize = br.ReadUInt32();
                        obj.KTID = br.ReadUInt32();
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
                            Type = (OBJDBPropertyType)br.ReadInt32(),
                            RowCount = br.ReadUInt32(),
                            PropertyKTID = br.ReadUInt32()
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
                        br.BaseStream.Position += 4 - mod;
                    }

                    objects.Add(obj);
                }

                KidsOdbObjectFile.Header = header;
                KidsOdbObjectFile.Objects = objects;
            }


        }

        private object ReadValue(BinaryReader br, OBJDBPropertyType type)
        {
            return type switch
            {
                OBJDBPropertyType.Bool => br.ReadByte() != 0,
                OBJDBPropertyType.Byte => br.ReadByte(),
                OBJDBPropertyType.Int16 => br.ReadInt16(),
                OBJDBPropertyType.UInt16 => br.ReadUInt16(),
                OBJDBPropertyType.Int32 => br.ReadInt32(),
                OBJDBPropertyType.UInt32 => br.ReadUInt32(),
                OBJDBPropertyType.Int64 => br.ReadInt64(),
                OBJDBPropertyType.UInt64 => br.ReadUInt64(),
                OBJDBPropertyType.Float32 => br.ReadSingle(),
                OBJDBPropertyType.Vector2 => new Vector2(br.ReadSingle(), br.ReadSingle()),
                OBJDBPropertyType.Vector3 => new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                OBJDBPropertyType.Vector4 => new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                _ => throw new Exception($"Unsupported ODB property type: {type}")
            };
        }

    }
}