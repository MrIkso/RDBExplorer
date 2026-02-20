namespace RDBExplorer.Core.G1T
{
    public static class G1TFormatUtils
    {
        public static int GetBytesPerBlock(G1TFormat format)
        {
            switch (format)
            {
                case G1TFormat.BC1_06:
                case G1TFormat.BC1_10:
                case G1TFormat.BC1_59:
                case G1TFormat.BC1_60:
                case G1TFormat.BC4_5C:
                case G1TFormat.BC4_63:
                    return 8;
                case G1TFormat.BC2_07:
                case G1TFormat.BC2_11:
                case G1TFormat.BC2_5A:
                case G1TFormat.BC2_61:
                case G1TFormat.BC3_08:
                case G1TFormat.BC3_12:
                case G1TFormat.BC3_5B:
                case G1TFormat.BC3_62:
                case G1TFormat.BC5_5D:
                case G1TFormat.BC5_64:
                case G1TFormat.BC6_5E:
                case G1TFormat.BC6_65:
                case G1TFormat.BC7_5F:
                case G1TFormat.BC7_66:
                    return 16;

                default:
                    return 0;
            }
        }

        public static int GetBytesPerPixel(G1TFormat format)
        {
            switch (format)
            {
                case G1TFormat.R8G8B8A8:
                case G1TFormat.B8G8R8A8:

                case G1TFormat.R8G8B8A8_09:
                case G1TFormat.B8G8R8A8_0A:
                case G1TFormat.RGBA8_UINT_67:
                case G1TFormat.RGBA8_UINT_74:
                    return 4;

                case G1TFormat.B5G6R5_UNORM_19:
                case G1TFormat.B5G5R5A1_UNORM_1A:
                case G1TFormat.D16:
                case G1TFormat.RG8_UNORM_73:
                case G1TFormat.RG16_FLOAT_69:
                case G1TFormat.R16_FLOAT_6A:
                    return 2;

                case G1TFormat.R8_UNORM_2A:
                case G1TFormat.A8_UNORM_0F:
                case G1TFormat.A8_UNORM_18:
                case G1TFormat.R8_UNORM_72:
                    return 1;

                case G1TFormat.R16G16B16A16_HALF_FLOAT:
                case G1TFormat.R16G16B16A16_HALF_FLOAT_0C:
                    return 8;

                case G1TFormat.R32G32B32A32_FLOAT:
                case G1TFormat.RGBA32_FLOAT_0D:
                    return 16;

                default:
                    return 4;
            }
        }

        public static int CalculateMipSize(G1TTexture tex, uint w, uint h)
        {
            int bytesPerBlock = GetBytesPerBlock(tex.Format);
            int bpp = GetBytesPerPixel(tex.Format);

            if (bytesPerBlock > 0)
            {
                uint blocksW = (w + 3) / 4;
                uint blocksH = (h + 3) / 4;

                int mipSize = (int)(blocksW * blocksH * bytesPerBlock);
                return Math.Max(mipSize, bytesPerBlock);
            }
            else
            {
                return (int)(w * h * bpp);
            }
        }

        public static bool IsCompressed(this G1TFormat format) => GetBytesPerBlock(format) > 0;
    }
}
