struct TextEntry {
    char text[64];
    u32 unkInt1;
    float unk1;
    float unk2;
    float unk3;
    float unk4;
    float unk5;
    float unk6;
    float unk7;
    float unk8;
    u32 unkInt;
};

struct TexInfo {
    u32 count;
    TextEntry entries[count];
};

TexInfo textInfo @0x0;