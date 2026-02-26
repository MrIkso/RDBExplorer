import std.mem;

struct KG1C {
    char magic[4];
    u32 version;
    u32 headerSize;
    u32 textLen;
    char text[textLen];
};

struct G1CX {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 dataStartPointer;
    u8 hederData [dataStartPointer - 16];
    u8 data[blockSize - dataStartPointer];
};

struct G1COXFile {
    KG1C kg1c;
    padding [std::mem::align_to(16, sizeof(this)) - sizeof(this)];
    G1CX g1cx;
};

G1COXFile g1cox @0x0;