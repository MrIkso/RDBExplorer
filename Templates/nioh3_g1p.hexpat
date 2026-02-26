import std.mem;

struct KG1P {
    char magic[4];
    u32 version;
    u32 headerSize;
    u32 textLen;
    char text[textLen];
};

struct G1PE{
    char magic[4];
    char version[4];
    u32 blockSize;
    u8 data[blockSize - 12];
};

struct G1PS {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 unk;
    u8 data[blockSize - 16];
};

struct G1PJ {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 unk;
    u8 data[blockSize - 16];
};

struct G1PG {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 unk;
    u8 data[blockSize - 16];
};

struct G1P {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 dataStartPointer;
    padding [dataStartPointer - 16];
};

struct G1PFile {
    KG1P kg1p;
    padding [std::mem::align_to(16, sizeof(this)) - sizeof(this)];
    G1P g1p;
    G1PS g1ps;
    G1PE g1pe;
    G1PJ g1pj;
    G1PG g1pg;
};

G1PFile g1p @0x0;