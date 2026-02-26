import std.mem;

// 48 bytes
struct Node {
    float unk1;
    float unk2;
    float unk3;
    float unk4;
    float unk5;
    float unk6;
    float unk7;
    float unk8;
    u32 unk9;
    u32 unk10;
    u32 unk11;
    u32 unk12;
};

// 3 * 2 bytes
struct Triangle {
    u16 x;
    u16 y;
    u16 z;
};

// 16 bytes
struct Extra{
    float unk1;
    float unk2;
    float unk3;
    u32 unk4;
};

struct HVB{
    char magic[4];
    char version[4];
    u32 pad2;
    u32 blockSize;
    u32 nodesCount ;
    u32 nodesOffset ;
    u32 triangleCount;
    u32 triangleOffset;
    u32 extraCount;
    u32 extraOffset;
    u32 pad;
    u32 hash;
    Node nodes[nodesCount] @ nodesOffset + sizeof(parent);
    Triangle triangles[triangleCount] @ triangleOffset + sizeof(parent);
    // aligned data by 16 byte   
    // padding [std::mem::align_to(16, sizeof(this)) - sizeof(this)];
    Extra extraData[extraCount] @ extraOffset +  sizeof(parent);
};

struct G1COFile {
    char magic[4];
    char version[4];
    u8 unk;
    u8 metadatSize;
    u8 numEntries;
    u8 dispathMode;
    u32 fileSize;
    HVB hvb[numEntries];
};

G1COFile g1co @0x0;