import std.mem;

struct KG1M {
    char magic[4];
    u32 version;
    u32 headerSize;
    u32 textLen;
    char text[textLen];
};

struct GResourceChunk{
    char magic[4];
    char version[4];
    u32 dataSize;
    u8 data [dataSize-12];
};

struct G1M {
    char magic[4];
    char version[4];
    u32 dataSize;
    u32 headerSize;
    
    u32 unk;
    u32 chunkCount;
    //u8 data [dataSize - headerSize];
    GResourceChunk chunks[chunkCount];
};

struct GMXM {
    char magic[4];
    char version[4];
    u32 dataSize;
    u32 elementCount;
    u32 headerSize;
    u32 depedencyList[elementCount -1];
    
    // its a g1m model
    G1M g1m[elementCount];
};

struct G1MXF {
    char magic[4];
    char version[4];
    u32 headerSize;
    u32 unk1;
    u32 unk2;
    u32 zero;
    GMXM gmxm;
};

struct G1MX {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 dataStartPointer;
   // u8 data[blockSize - dataStartPointer];
    G1MXF g1mxf;
};

struct G1MXFile {
    KG1M kgM;
    padding [std::mem::align_to(16, sizeof(this)) - sizeof(this)];
    G1MX g1mx;
};

G1MXFile g1mx @0x0;