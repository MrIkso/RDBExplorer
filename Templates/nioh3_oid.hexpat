struct OidEntry {
    u32 index; 
    u64 ktid; 
};

struct OID {
    u32 unknown;
    OidEntry entries[100];
};

OID oid @0x0;