using OpenTK.Mathematics;

namespace RDBExplorer.Core.Formats.G1M;
// Vertex Buffer helper
public class G1MVertexBufferInternal
{
    public byte[] Data;
    public int Stride;

    public G1MVertexBufferInternal(byte[] bufferData, int stride)
    {
        Data = bufferData;
        Stride = stride;
    }

    public Vector3 ReadVec3(int o, G1MDataFormat fmt)
    {
        if (fmt == G1MDataFormat.R16G16_FLOAT || fmt == G1MDataFormat.R16G16B16A16_FLOAT)
            return new Vector3(ReadHalf(o), ReadHalf(o + 2), ReadHalf(o + 4));

        if (fmt == G1MDataFormat.R16G16B16A16_UINT) // R16G16B16A16
            return new Vector3(
                BitConverter.ToInt16(Data, o),
                BitConverter.ToInt16(Data, o + 2),
                BitConverter.ToInt16(Data, o + 4));

        return new Vector3(ReadFloat(o), ReadFloat(o + 4), ReadFloat(o + 8));
    }

    public Vector2 ReadVec2(int o, G1MDataFormat fmt)
    {
        if (fmt == G1MDataFormat.R16G16_FLOAT || fmt == G1MDataFormat.R16G16B16A16_FLOAT)
            return new Vector2(ReadHalf(o), ReadHalf(o + 2));
        return new Vector2(ReadFloat(o), ReadFloat(o + 4));
    }

    public Vector4 ReadVec4(int offset, EG1MGVADatatype fmt)
    {
        ushort SafeReadUI16(int offset)
        {
            if (offset + 2 > Data.Length)
                return 0;
            return BitConverter.ToUInt16(Data, offset);
        }

        switch (fmt)
        {
            case EG1MGVADatatype.VADataType_UByte_x4:
                return new Vector4(Data[offset], Data[offset + 1], Data[offset + 2], Data[offset + 3]);
            case EG1MGVADatatype.VADataType_UShort_x4:
                ushort rawX = BitConverter.ToUInt16(Data, offset);
                ushort rawY = BitConverter.ToUInt16(Data, offset + 2);
                ushort rawZ = BitConverter.ToUInt16(Data, offset + 4);
                ushort rawW = BitConverter.ToUInt16(Data, offset + 6);

                float nx = rawX / 65535.0f;
                float ny = rawY / 65535.0f;
                float nz = rawZ / 65535.0f;
                float nw = rawW / 65535.0f;
                return new Vector4(nx, ny, nz, nw);

            case EG1MGVADatatype.VADataType_NormUByte_x4:
                return new Vector4(Data[offset] / 255f, Data[offset + 1] / 255f, Data[offset + 2] / 255f, Data[offset + 3] / 255f);

            case EG1MGVADatatype.VADataType_HalfFloat_x4:
                return new Vector4(ReadHalf(offset), ReadHalf(offset + 2), ReadHalf(offset + 4), ReadHalf(offset + 6));

            default: // VADataType_Float_x4
                return new Vector4(ReadFloat(offset), ReadFloat(offset + 4), ReadFloat(offset + 8), ReadFloat(offset + 12));
        }
    }

    private float ReadFloat(int o)
        => (o + 4 <= Data.Length) ? BitConverter.ToSingle(Data, o) : 0f;

    private float ReadHalf(int o)
    {
        if (o + 2 > Data.Length)
            return 0f;
        ushort h = BitConverter.ToUInt16(Data, o);
        int s = (h >> 15) & 0x01;
        int e = (h >> 10) & 0x1F;
        int m = h & 0x3FF;
        float sign = (s == 1) ? -1f : 1f;
        if (e == 0)
            return sign * (float)(Math.Pow(2, -14) * (m / 1024.0));
        if (e == 31)
            return m == 0 ? (sign * float.PositiveInfinity) : float.NaN;
        return sign * (float)(Math.Pow(2, e - 15) * (1.0 + m / 1024.0));
    }
}
