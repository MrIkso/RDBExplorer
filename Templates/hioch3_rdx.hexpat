struct idxFile {
    u16 index;
    u16 pad;
    u32 hashFileName;
};

idxFile idx[4] @ 0x0;