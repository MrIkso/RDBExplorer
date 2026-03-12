namespace RDBExplorer.Core.Formats.G1M;
public enum G1MDataFormat : ushort
{
    R32_FLOAT = 0,
    R32G32_FLOAT = 1,
    R32G32B32_FLOAT = 2,
    R32G32B32A32_FLOAT = 3,
    R8G8B8A8_UINT = 5,
    R16G16B16A16_UINT = 7,
    R32G32B32A32_UINT = 10,
    R16G16_FLOAT = 10,
    R16G16B16A16_FLOAT = 11,
    R8G8B8A8_UNORM = 13
}

public enum EG1MGVADatatype : ushort
{
    VADataType_Float_x1 = 0x00,
    VADataType_Float_x2 = 0x01,
    VADataType_Float_x3 = 0x02,
    VADataType_Float_x4 = 0x03,
    VADataType_UByte_x4 = 0x05,
    VADataType_UShort_x4 = 0x07,
    VADataType_UInt_x4 = 0x09, //Need confirmation
    VADataType_HalfFloat_x2 = 0x0A,
    VADataType_HalfFloat_x4 = 0x0B,
    VADataType_NormUByte_x4 = 0x0D,
    VADataType_Dummy = 0xFF
}

public enum EG1MGVASemantic : byte
{
    Position = 0x00,
    JointWeight,
    JointIndex,
    Normal,
    PSize,
    UV,
    Tangent,
    Binormal,
    TessalationFactor,
    PosTransform,
    Color,
    Fog,
    Depth,
    Sample
};