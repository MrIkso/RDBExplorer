#pragma array_limit 999999999999
#pragma pattern_limit 999999999999

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
    u64 dataSize;
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
    u8 metadata[header.dataSize];
    
    // align
    std::mem::Bytes<std::mem::align_to(4, sizeof(this)) - sizeof(this)> pad;
};

struct RDBHeader {
    char magic[4];
    char version[4];
    u32 headerSize;
    u32 unk;
    u32 filesCount;
    u32 systemID;
    char path[8];
};

struct RdbFile {
    RDBHeader header;
    KRDIBlock entries[header.filesCount];
};

RdbFile rdb @ 0x0;