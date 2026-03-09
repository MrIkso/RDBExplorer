using System.Runtime.InteropServices;

namespace RDBExplorer.Core.Formats.KSCL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 76)]
    public struct KSCLHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public char[] Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public char[] Version;
        public uint FileSize;
        public uint SectionCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] 
        public ushort[] MemoryFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] PoolSizes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public uint[] UnkPoolSizes;
        public uint Padding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
    public struct SectionHeader
    {
        public SectionType TypeId;
        public uint TotalSize;
        public uint DictionarySize;
        public uint SecionTableSize;
        public ushort Flags;
        public ushort ItemCount;
        public uint Unk;
        public uint SecionTablePointer;
    }
}
