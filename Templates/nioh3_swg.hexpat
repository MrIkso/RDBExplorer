struct Entry {
    u32 ktid;
    float unk2;
    float unk3;
    float unk4;
    float unk5;
    u32 unk6;
    u32 unk7;
    u32 unk8;
    u32 unk9;
    u32 unk10;
    u32 unk11;
    u32 unk12;
    u32 unk13;
    u32 unk14;
    float unk15;
    u32 unk16;
    u16 index1;
    u16 index2;
    u32 unk18;
};

struct Header {
    char magic[4];
    u32 blockSize;
    u32 entryCount;
    char roupName[8]; 
    float globalScale;
    u32 unk1;
    u32 unk2;
    u32 unk3;
};

struct SWGFile {
    Header header;
    Entry entries [header.entryCount];
};

SWGFile swg @0x0;