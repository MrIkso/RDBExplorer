struct PhysicsEntry {
    u32 g1mId;
    u32 xsiId;
};

struct MaterialEntry {
    u32 hash;
    u32 idCount;
    u32 ids[idCount];
};

struct MTLHeader {
    u32 numNames;
    u32 numMaterials;
    u32 numCloths;
    u32 numPonyTails;
};

struct MTL {
    MTLHeader header;
    MaterialEntry materisals[header.numNames];
    PhysicsEntry cloths[header.numCloths];
    PhysicsEntry ponytails[header.numPonyTails];
};

MTL mtl  @ 0x0;