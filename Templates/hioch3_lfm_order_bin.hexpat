import std.mem;

struct Header {
    char magic[4];
    u32 version;
    u32 fileCount;
    u32 headerSize;
    u32 unk1;
    u32 rootDirPathOffset;
    u32 allHeaderSize;
    u32 unk2;
};

struct StringOffsets {
    u32 unk1;
    u32 index;
    u32 offset;
    
    char name [while(std::mem::read_unsigned($, 1) != 0x0)] @ offset;
};

struct LFMOFile {
    Header header;
    u32 unk1;
    u32 rootDirPathOffset;
    char rootDirName [while(std::mem::read_unsigned($, 1) != 0x0)] @ rootDirPathOffset;
    StringOffsets offsets[header.fileCount];
};

LFMOFile lfmo @ 0x0;