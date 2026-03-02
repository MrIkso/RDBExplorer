### KTGL Resource Types Table

| Resource Type | Extension | Parser | Description | Magic |
| :--- | :--- | :---: | :--- |  :--- |
| **AssetData** | `.srsa` | ❌ | Audio data (sound effects, sound banks) | `0x41535253`
| **BinaryFile** | `.efpl` | ❌ | Binary data, misc files |
| **CSVFile** | `.mit` | ❌  | |
| **EffectData** | `.g1e` | ❌ | Visual effect parameters (particles, emitters, timings) | `0x58463147`
| **EffectMeshData** | `.g1em` | ❌ | Geometry (meshes) used for special effects | `0x4d453147`
| **EffectShapeMeshData** | `.g1es` | ❌ | Shape-specific meshes for visual effects | `0x53453147`
| **FPoseData** | `.g1fpose` | ❌  | Library of fixed character poses | `0x3146504f`
| **FRAnimationData** | `.g1frani` | ❌ | Facial or skeletal bone-based animation data |  `0x31465241`
| **G1AFile** | `.g1a` | ❌  | Main skeletal animation files | `0x5f413247`
| **G1COFile** | `.g1co` | ✅  | Collision data | `0x4f433147`
| **G1COXFile** | `.g1cox` | ❌  | Extended Collision data | `0x58433147`
| **G1HFile** | `.g1h` | ❌  |  | `0x5f483147`
| **G1IIFile** | `.gii` | ❌  |  | `0x49493147`
| **G1MXFile** | `.g1mx` | ✅ | Model pack container for specific scenes/environments | `0x4d314f4b`
| **G1NFile** | `.g1n` | ❌ | Font glyph data and font texure | `0x5f4e3147`
| **G1PFile** | `.g1p` | ❌ |  | `0x5031474b`
| **G1SFile** | `.g1s` | ❌ | Compiled shaders | `0x5f533247`
| **GlobalConfiguration** | `.sgcbin` | ❌ | Global system settings and sound pack configurations | `0x43475253`
| **KSCLFile** | `.kscl` | ❌ | Compiled UI layouts |  `0x4c43534b`
| **KTIDFileBinary** | `.ktid` | ✅ | Resource ID registry (linkage between hashes and files) |
| **LandscapeQuadtree** | `.lsqtree` | ✅ | Spatial hierarchy for landscape optimization |
| **MaterialGroupBindTableBinaryFile** | `.mtl` | ✅ | Material list and texture-to-shader register bindings |
| **ModelData** | `.g1m` | ❌ | Main 3D model | `0x5f4d3147`
| **MotionMatchingDatabase** | `.mmdb` | ❌ | Procedural animation database | `0x6d6d6462`
| **ObjectDatabaseFile** | `.kidsobjdb` | ✅ | Main object property database | `0x5f444f4b`
| **OBOROStaticResourceBinaryFile** | `.oboro` | ✅ | Static environment data|
| **OIDBindTableBinaryFile** | `.oid` | ✅ | Object ID bind table |
| **OIDSQTBindTableBinaryFile** | `.oidsq` | ✅ | Extended OID table with Scale-Quat-Translation data |
| **PartsModelGroupBindTableBinaryFile** | `.grp` | ✅ | Grouping table for modular character parts |
| **RBFData** | `.grbf` | ✅ | |  `0x46425247`
| **RenderGraphFile** | `.kidsrender` | ❌ | | `0x5f52474b`
| **RigBinFile** | `.rigbin` | ❌ | Binary rigging data for bone constraints | `0x42474952`
| **River2BakedGeometry** | `.rbg` | ❌ | |  `0x32523147`
| **ShaderBindTableBinaryFile** | `.sid` | ✅ | | 
| **StaticScreenLayoutTexInfoFile** | `.texinfo` | ✅ | UI sprite UV coordinates on texture atlases |
| **StreamAssetDataFile** | `.srst` | ❌ | Streaming audio | `0x54535253`
| **StreamingMeshletModelData** | `.g1ms` | ❌ | Streaming 3D model |`0x5f4d3147`
| **StreamingTexContext** | `.g1ts` | ✅ | Streaming texture | `0x47543147`
| **SwingData** | `.swg` | ✅ | "Swing" physics parameters (hair, cloth, soft body) | `0x53574751`
| **TexContext** | `.g1t` | ✅ | Texture container | `0x47543147`
| **TexStageTableBinaryFile** | `.kts` | ✅ | Texture stage configuration | `0x4753544b`