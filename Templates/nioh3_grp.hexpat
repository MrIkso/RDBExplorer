struct GRPEntry {
    u64 ktid;
    u32 type1;
    u32 unk1;
    u32 unk2;
    u32 unk3;
    u32 unk4;
    u32 unk5; 
};

struct GRP {
    GRPEntry entries[1];
};

GRP grp @0x0;