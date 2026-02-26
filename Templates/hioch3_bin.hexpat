import std.mem;

struct Header{
    char magic[4];
    u32 version;
    u64 archiveCount;
    u64 containerSize;
    u64 align;
};

struct LFMArchiveEntry {
    u64 offset;
    u64 unCompressedSize;
    u64 compressedSize;
    u64 isCompressed;    
    std::mem::Bytes<compressedSize> data @ offset;
};

struct LFMArchive {
    Header header;
    LFMArchiveEntry entries[header.archiveCount];
};

LFMArchive archive @ 0x0;