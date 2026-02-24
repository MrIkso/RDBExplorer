using RDBExplorer.Core.Models;

namespace RDBExplorer.Core.Formats.G1T
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

        private static readonly int[] PointSizes = {
            32, 32, 32, 64, 128, 32, 4, 8, 8, 32, 32, 32, 64, 128, 0, 8,
            4, 8, 8, 32, 16, 16, 16, 16, 8, 16, 16, 16, 16, 16, 16, 32,
            32, 32, 32, 32, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            32, 64, 32, 64, 0, 0, 16, 0, 0, 0, 0, 0, 64, 64, 32, 32,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 8, 8, 4, 8, 8, 8,
            4, 8, 8, 4, 8, 8, 8, 32, 16, 32, 16, 32, 64, 0, 0, 0,
            0, 0, 8, 16, 32, 16, 32, 16, 32, 0, 0, 0, 0
        };

        public static int CalculateMipSize(G1TTexture tex, uint w, uint h, KoeiPlatform platform)
        {
            byte fmt = (byte)tex.Format;
            if (fmt >= PointSizes.Length)
            {
                return 0;
            }

            int pointSize = PointSizes[fmt];
            if (pointSize == 0)
            {
                return 0;
            }
            int blockSizeInBytes;
            if (pointSize >= 32)
            {
                blockSizeInBytes = pointSize / 8;
            }
            else
            {
                blockSizeInBytes = pointSize * 16 / 8;
            }

            long calculatedSize = (long)w * h * pointSize / 8;
            int mipSize = (int)Math.Max(calculatedSize, blockSizeInBytes);

            int alignment = 0;
            if (platform == KoeiPlatform.PS4)
            {
                alignment = 1024;
            }
            else if (platform == KoeiPlatform.WinDX12)
            {
                alignment = 4096;
            }
            
            if (alignment > 0)
            {
                mipSize = mipSize + alignment - 1 & ~(alignment - 1);
            }

            return mipSize;
        }

        public static bool IsCompressed(G1TFormat format)
        {
            byte f = (byte)format;
            if (f >= PointSizes.Length)
            {
                return false;
            }
            return PointSizes[f] < 32 && PointSizes[f] != 0;
        }
    }
}
