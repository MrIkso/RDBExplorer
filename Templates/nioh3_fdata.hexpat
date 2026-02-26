import std.mem;

//12 byte
struct ParamEntry {
    u32 type;      
    u32 unk;    
    u32 hashName;      
};

// size 56 byte
struct KRDIHeader{
    char magic[4];
    char version[4];
    u64 allSize;
    u64 compressedSize;
    u64 uncompressedSize;
    u32 paramDataSize;
    u32 hashName;
    u32 hashType;
    u32 flags;
    u32 resourceId;
    u32 paramCount;
};

struct KRDIBlock{
    KRDIHeader header;
    ParamEntry params[header.paramCount];
    u8 paramData[header.paramDataSize];
    std::mem::Bytes<header.compressedSize> data;
};

KRDIBlock block @ 0x10BB3C10;