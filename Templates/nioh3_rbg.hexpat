struct G1R2 {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 headerSize;
    u32 unk1;
};

struct G1FI {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 data[(blockSize - 12)/4];
};

struct G1GEEntry {
u8 data [64];
};
struct G1GE {
    char magic[4];
    char version[4];
    u32 blockSize;
    char mode[8];
    u32 unk1;
    u32 unk2;
    u32 unk4;
    u32 unk5;
    u32 unk6;
    u64 count;
    G1GEEntry entries[count];
    u8 data[blockSize - sizeof(entries) - 48];
};

struct G1SI {
    char magic[4];
    char version[4];
    u32 blockSize;
    u8 data[blockSize - 16 - 12];
};

struct RBG {
    G1R2 g1rHeader;
    G1FI g1fiBlock;
    G1GE g1geBlock;
    G1SI g1siBlock;
};

RBG rgb @0x0;