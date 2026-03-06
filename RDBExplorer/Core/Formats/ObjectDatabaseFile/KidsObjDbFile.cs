using RDBExplorer.Core.Models;
using RDBExplorer.Utils.JsonConverters;
using System.Text;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.ObjectDatabaseFile
{
    public enum OBJDBPropertyType : uint
    {
        Bool = 0,
        Byte = 1,
        Int16 = 2,
        UInt16 = 3,
        Int32 = 4,
        UInt32 = 5,
        Int64 = 6,
        UInt64 = 7,
        Float32 = 8,
        Vector4 = 10,
        Vector2 = 12,
        Vector3 = 13
    }

    public class KidsObjDbHeader
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint HeaderSize { get; set; }
        public KoeiPlatform Platform { get; set; }
        public uint NumEntries { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint NameKTID { get; set; }
        public uint TotalSize { get; set; }
    }

    [JsonConverter(typeof(KidsOdbColumnConverter))]
    public class KidsOdbColumn
    {
        public OBJDBPropertyType Type { get; set; }
        public uint RowCount { get; set; }
       
        public uint PropertyKTID { get; set; }

        public string PropertyName => KidsObjNameTypeIDHelper.Instance.GetPropertyName(PropertyKTID);

      
        public List<object> Values { get; set; } = new List<object>();

        public override string ToString() => GetFormattedValue();

        private string GetFormattedValue()
        {
            if (Values == null || Values.Count == 0) 
                return "Empty";

            if (Type == OBJDBPropertyType.Byte)
            {
                byte[] bytes = Values.Cast<byte>().ToArray();
                if (bytes.Length > 0)
                {
                    string raw = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
                    return raw.Replace("\0", ", ");
                }
            }

            if (Values.Count > 1)
            {
                return $"Array[{Values.Count}] {string.Join(", ", Values)}...";
            }

            return Values[0]?.ToString() ?? "null";
        }
    }

    public class KidsOdbObject
    {
        public string Magic { get; set; }

        public string Version { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KTID { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint TypeInfoKTID { get; set; }
        public string TypeName => KidsObjNameTypeIDHelper.Instance.GetLocalName(TypeInfoKTID);
        public string TypeFullName => KidsObjNameTypeIDHelper.Instance.GetFullName(TypeInfoKTID);
        public bool IsReference { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint? ParentObjectFileKtid { get; set; }
        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint? ParentObjectKtid { get; set; }
        public uint EntrySize { get; set; }
        public List<KidsOdbColumn> Columns { get; set; } = new List<KidsOdbColumn>();
       
    }

    public class KidsOdbObjectFile
    {
        public KidsObjDbHeader Header { get; set; }
        public List<KidsOdbObject> Objects { get; set; } = new List<KidsOdbObject>();

    }
}
