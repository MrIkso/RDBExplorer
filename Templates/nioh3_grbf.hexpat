struct RBFEntry {
    u32 blockSize;
    u32 data[(blockSize - 4)/4];
};

struct Header {
    char magic[4];
    char version[4];
    u32 blockSize;
    u32 elementCount;
};

struct GRBF {
    Header header;
    RBFEntry entries[header.elementCount];
};

GRBF grbf @ 0x0;