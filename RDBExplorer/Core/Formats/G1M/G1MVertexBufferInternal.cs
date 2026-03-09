using OpenTK.Mathematics;

namespace RDBExplorer.Core.Formats.G1M;
// Vertex Buffer helper
public class G1MVertexBufferInternal
{
    public byte[] Data;
    public int Stride;

    public G1MVertexBufferInternal(byte[] d, int s) { Data = d; Stride = s; }

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

    public Vector4 ReadVec4(int o, G1MDataFormat fmt)
    {
        ushort SafeReadUI16(int offset)
        {
            if (offset + 2 > Data.Length) return 0;
            return BitConverter.ToUInt16(Data, offset);
        }


        switch (fmt)
        {
            case G1MDataFormat.R8G8B8A8_UINT:  // R8G8B8A8_UINT  – raw byte values
                return new Vector4(Data[o], Data[o + 1], Data[o + 2], Data[o + 3]);
            case G1MDataFormat.R16G16B16A16_UINT:
                return new Vector4(SafeReadUI16(o),SafeReadUI16(o + 2),SafeReadUI16(o + 4), SafeReadUI16(o + 6));

            case G1MDataFormat.R8G8B8A8_UNORM: // R8G8B8A8_UNORM  – normalised [0,1]
                return new Vector4(Data[o] / 255f, Data[o + 1] / 255f, Data[o + 2] / 255f, Data[o + 3] / 255f);

            case G1MDataFormat.R16G16B16A16_FLOAT: // R16G16B16A16_FLOAT
                return new Vector4(ReadHalf(o), ReadHalf(o + 2), ReadHalf(o + 4), ReadHalf(o + 6));

            default: // R32G32B32A32_FLOAT
                return new Vector4(ReadFloat(o), ReadFloat(o + 4), ReadFloat(o + 8), ReadFloat(o + 12));
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
