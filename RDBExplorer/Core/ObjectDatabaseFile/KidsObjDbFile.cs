using RDBExplorer.Core.Models;
using RDBExplorer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.ObjectDatabaseFile
{
    public enum KidsOdbType : int
    {
        Int8 = 0,
        Uint8 = 1,
        Int16 = 2,
        Uint16 = 3,
        Int32 = 4,
        Uint32 = 5,
        Float = 8,
        Vector4 = 10,
        Vector2 = 12,
        Vector3 = 13,
    }

    public class KidsObjDbHeader
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint HeaderSize { get; set; }
        public KoeiPlatform Platform { get; set; }
        public uint NumEntries { get; set; }
        public uint NameFile { get; set; }
        public uint FileSize { get; set; }
    }

    public class KidsOdbColumn
    {
        public uint NameHash { get; set; }
        public KidsOdbType Type { get; set; }
        public uint RowCount { get; set; }
        public List<object> Values { get; set; } = new List<object>();

        public override string ToString()
        {
            if (Type != KidsOdbType.Uint8 || Values.Count == 0) 
                return null;
            byte[] bytes = new byte[Values.Count];
            for (int i = 0; i < Values.Count; i++) 
                bytes[i] = (byte)Values[i];

            return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        }
    }

    public class KidsOdbObject
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint NameHash { get; set; }
        public uint TypeKtid { get; set; }
        public uint EntrySize { get; set; }
        public bool IsReference { get; set; }
        public uint ParentObjectFileKtid { get; set; }
        public uint ParentObjectKtid { get; set; }
        public List<KidsOdbColumn> Columns { get; set; } = new List<KidsOdbColumn>();

        public string TypeName => KidsObjNameTypeIDHelper.Instance.GetLocalName(TypeKtid);

        public string TypeFullName => KidsObjNameTypeIDHelper.Instance.GetFullName(TypeKtid);
    }
}
