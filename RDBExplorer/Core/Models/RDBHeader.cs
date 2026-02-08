using RDBExplorer.Utils;
using System;
using System.Collections.Generic;
namespace RDBExplorer.Core.Models
{
    public class RDBHeader
    {
        public string Magic; // _DRK
        public int Version; // 0000
        public int HeaderSize;      
        public int SystemId;       
        public int FileCount;       
        public uint DatabaseId;   
        public string FolderPath;
    }

    // RdbFlags structure (bit fields)
    public enum RDBFlags : uint
    {
        StorageMask = 0x000F0000,
        StorageExternal = 0x00010000,
        StorageInternal = 0x00020000,

        CompressionNone = 0,
        CompressionZlib = 1,      // 0x00100000 >> 20
        CompressionLz4 = 2,       // 0x00200000 >> 20
        CompressionEncrypted = 3, // 0x00300000 >> 20
        CompressionExtended = 4,  // 0x00400000 >> 20
    }

    [Flags]
    public enum RDBFlagsNew : uint
    {
        External = 0xC01,
        Internal = 0x401,
    }

    public class EntryLocation
    {
        public RDBFlagsNew NewFlags { get; set; }
        public string ContainerPath { get; set; }
        public ulong Offset { get; set; }
        public ulong SizeInContainer { get; set; }
        public int FDataId { get; set; }

        public override string ToString()
        {
            return ContainerPath;
        }
    }

    public class RDBEntry
    {
        public string Magic { get; set; }
        public uint Version { get; set; }
        public long EntrySize { get; set; }
        public long DataSize { get; set; }
        public long FileSize { get; set; }
        public uint EntryType { get; set; }
        public uint FileKtid { get; set; }
        public uint TypeInfoKtid { get; set; }
        public RDBFlags Flags { get; set; }
        public byte[] UnkContent { get; set; }
        public long EntryOffsetInRDB { get; set; }
        public EntryLocation Location { get; set; } = new();
        public string Name => TypeIDHelper.Instance.GetFileName(FileKtid, TypeInfoKtid);
        public string TypeName => TypeIDHelper.GetTypeName(TypeInfoKtid);
    }
}
