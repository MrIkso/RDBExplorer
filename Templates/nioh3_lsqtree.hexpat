struct Entry {
    u16 unk1;
    u16 unk2;
    u16 unk3;
    u16 index;
    u32 unk4;
    u32 unk5;
    u16 unk6;
    u16 unk7;
    u32 ktid;
    u32 unk8;
    u32 unk9;
};

struct Header {
    u32 magic;
    u32 version;
    u32 fileSize;
    u32 ktid;
    u16 count;
    u16 unk3;
    Entry datas[count];
};

Header  header @0x0;