struct Entry {
    u32 hash1;
    u32 hash2;
    u32 hash3;
    u32 hash4;
};

struct Sid {
    u32 zero;
    u32 version;
    u32 count;  
    Entry entries[count];
};

Sid sid @ 0x0;