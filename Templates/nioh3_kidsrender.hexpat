import std.mem;

struct KidsEntry {
    char magic[4];
    char version[4];
    u32 blockSize;
    u8 data [blockSize - 12];
    padding [std::mem::align_to(4, sizeof(this)) - sizeof(this)];
    
};

struct Header {
    char magic[4];
    char version[4];
    u32 blockSize;    
    u32 unk1;
    u32 elementCount;
    u32 ktId1;
    u32 unk2;
    u32 ktid2;
};

struct KidsRenderFile {
    Header header;
    KidsEntry entries[header.elementCount];
};
KidsRenderFile kgr @0x0;