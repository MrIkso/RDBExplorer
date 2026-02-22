using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Models
{
    public enum KTFileType : uint
    {
        // Common files
        TexContext = 0xAFBEC60C,
        StreamingTexContext = 0xAD57EBBA,
        ModelData = 0x563BDEF1,
        G1NFile = 0x786DCD84,
        G1MXFile = 0x17614AF5,
        G1AFile = 0x6FA91671,
        G1SFile = 0x7BCD279F,
        G1PFile = 0x79C724C2,
        G1COFile = 0x54738C76,
        G1COXFile = 0xA8D88566,
        G1HFile = 0x7461C7CA,
        G1IIFile = 0xDB0AE0AA,
        EffectData = 0xB097D41F,
        EffectMeshData = 0x4D0102AC,
        EffectShapeMeshData = 0x1A6300FD,
        FRAnimationData = 0x2BCC0C02,
        FPoseData = 0x32AC9403,

        // System DB
        ObjectDatabaseFile = 0x20A6A0BB,
        NameDatabaseFile = 0xBF6B52C7,
        TaskGraphFile = 0x1FDCAA40,
        RenderGraphFile = 0xB1630F51,
        KTIDFile = 0xBE144B78,
        KTIDFileBinary = 0x8E39AA37,
        GlobalConfiguration = 0xB0A14534,
        OBOROStaticResourceBinaryFile = 0x8D735C52,

        // Bindings and tables
        OIDBindTableBinaryFile = 0x1AB40AE8,
        OIDFile = 0xDBCB74A9,
        OIDBindTableBinaryFileEx = 0xE6A3C3BB,
        OIDExFile = 0x9CB3A4B6,
        OIDSQTBindTableBinaryFile = 0x753AA042,
        MaterialGroupBindTableBinaryFile = 0xB340861A,
        PartsModelGroupBindTableBinaryFile = 0x56EFE45C,
        GroupFile = 0xBBF9B49D,
        RigBinFile = 0x27BC54B7,

        // Scripts and collisions
        KSCLFile = 0x5599AA51,
        KTSFile = 0x4F16D0EF,
        TexStageTableBinaryFile = 0xED410290,

        // UI and text
        G2NFile = 0xA1BDB205,
        G2NGlyphSetFile = 0x96C74B4F,
        ScreenLayoutColorTableBinaryFile = 0xC9D883C2,
        ScreenLayoutShapeInfoFile = 0xF13845EF,
        StaticScreenLayoutTexInfoFile = 0xF20DE437,

        // Audio and video
        AssetData = 0xBBD39F2D,
        StreamAssetDataFile = 0x0D34474D,
        ShaderBindTableBinaryFile = 0x133D2C3B,
        VideoStreamset = 0xA027E46B,
        StreamingMeshletModelData = 0xBEF563DD,

        // Others
        KTF2File = 0x5B2970FC,
        BinaryFile = 0xD7F47FB1,
        RBFData = 0x193D2E44,
        River2BakedGeometry = 0x4638B72D,
        SwingData = 0x5C3E543C,
        LandscapeQuadtree = 0x82945A44,
        MotionMatchingDatabase = 0xCBFD49B2,
        MITFile = 0x0BD05B27,
        CSVFile = 0x6DBD6EA6,
        OIDBindTable = 0xF02F31AB,

        Unknown = 0xFFFFFFFF
    }
}
