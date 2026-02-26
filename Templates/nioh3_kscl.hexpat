struct Block {
u32 id;
u32 offset;
u32 size;
u32 unk[4];
};
    
struct KSCL_2600 {
    char magic[4];          // "LCSK"
    char version[4];        // "2600"
    u32 fileSize;
    u32 blockCount;   
    Block blocks [blockCount]; 
};
   
KSCL_2600 kscl @ 0x0;