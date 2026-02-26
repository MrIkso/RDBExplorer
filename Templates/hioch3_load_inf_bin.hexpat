struct InfEntry {
    u32 index;
    u32 unCompressedSize;
    u32 isCompress; 
};

struct InfFile {
    u32 fileCount;
    InfEntry entries[fileCount];
};

InfFile infFile @ 0x0;