struct Entry {
    u32 unk1;
    u32 unk2;
    u32 unk3;
};

struct Oboro {
    u32 count;
    Entry entries[count];
    u8 types[count];
};

Oboro oboro @0x0;