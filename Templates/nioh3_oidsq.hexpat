struct OidSQEntry {
    u32 index;
    u32 ktid; 
    u32 unk;
    u32  unk1;
    u32  unk2;
    u32  unk3;
    u32 unk4;
    u32  unk5;
    u32  unk6;
    u32  unk7;
    u32  unk8;
    u32  ktid1;
    u32  ktid2;
    u32  ktid3;
};

struct OIDSQ {
    u32 count;
    OidSQEntry entries[count + 1];
};

OIDSQ oid @0x0;