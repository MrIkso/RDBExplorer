using AssetRipper.TextureDecoder.Bc;
using AssetRipper.TextureDecoder.Rgb;
using AssetRipper.TextureDecoder.Rgb.Formats;
using BCnEncoder.Shared.ImageFiles;
using RDBExplorer.Core.G1T;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;

namespace RDBExplorer.Utils
{
    public class TextureConverter
    {
        /// <summary>
        /// Сonverts 8-bit Alpha (A8) to 32-bit RGBA8.
        ///  R, G, B channels are set to 255.
        ///  </summary>
        public static byte[] ConvertA8ToRgba8(byte[] a8Data, int width, int height)
        {
            int pixelCount = width * height;
            if (a8Data.Length != pixelCount)
            {
                throw new ArgumentException("Incorrect input data size for A8.");
            }

            byte[] rgba8Data = new byte[pixelCount * 4];
            int a8Index = 0;
            for (int i = 0; i < rgba8Data.Length; i += 4)
            {
                byte alpha = a8Data[a8Index++];
                rgba8Data[i] = 255;     // Red
                rgba8Data[i + 1] = 255; // Green
                rgba8Data[i + 2] = 255; // Blue
                rgba8Data[i + 3] = alpha;  // Alpha
            }
            return rgba8Data;
        }

        /// <summary>
        /// Converts 16-bit Luminance-Alpha (LA8) to 32-bit RGBA8.
        /// R, G, B channels are set to the Luminance value.
        /// </summary>
        public static byte[] ConvertLa8ToRgba8(byte[] la8Data, int width, int height)
        {
            int pixelCount = width * height;
            if (la8Data.Length != pixelCount * 2)
            {
                throw new ArgumentException("Incorrect input data size for LA8.");
            }

            byte[] rgba8Data = new byte[pixelCount * 4];
            int laIndex = 0;
            for (int i = 0; i < rgba8Data.Length; i += 4)
            {
                byte luminance = la8Data[laIndex++];
                byte alpha = la8Data[laIndex++];
                rgba8Data[i] = luminance; // Red
                rgba8Data[i + 1] = luminance; // Green
                rgba8Data[i + 2] = luminance; // Blue
                rgba8Data[i + 3] = alpha;     // Alpha
            }
            return rgba8Data;
        }

        public static Bitmap ConvertA8ToBitmap(byte[] a8Data, int width, int height)
        {
            int pixelCount = width * height;
            if (a8Data.Length != pixelCount)
            {
                throw new ArgumentException("Incorrect input data size for A8.");
            }
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            int bytesPerPixel = 4;
            byte[] bgraData = new byte[width * height * bytesPerPixel];
            for (int i = 0; i < pixelCount; i++)
            {
                byte alpha = a8Data[i];
                int offset = i * bytesPerPixel;
                bgraData[offset] = 255; // Blue
                bgraData[offset + 1] = 255; // Green
                bgraData[offset + 2] = 255; // Red
                bgraData[offset + 3] = alpha; // Alpha
            }
            Marshal.Copy(bgraData, 0, bmpData.Scan0, bgraData.Length);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        public static void SaveA8AsPng(byte[] a8Data, int width, int height, string outputPath)
        {
            ConvertA8ToBitmap(a8Data, width, height).Save(outputPath, ImageFormat.Png);
        }

        public static byte[] ConvertPngToAlpha8(string pngFilePath)
        {
            if (!File.Exists(pngFilePath))
            {
                throw new FileNotFoundException("PNG файл не знайдено.", pngFilePath);
            }

            using (var bitmap = new Bitmap(pngFilePath))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                byte[] alpha8Data = new byte[width * height];

                BitmapData bmpData = bitmap.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    byte[] rowBuffer = new byte[bmpData.Stride];
                    IntPtr currentRowPtr = bmpData.Scan0;

                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(currentRowPtr, rowBuffer, 0, bmpData.Stride);

                        for (int x = 0; x < width; x++)
                        {
                            int sourceIndex = x * 4;
                            byte alpha = rowBuffer[sourceIndex + 3];
                            int destIndex = y * width + x;
                            alpha8Data[destIndex] = alpha;
                        }
                        currentRowPtr += bmpData.Stride;
                    }
                }
                finally
                {
                    bitmap.UnlockBits(bmpData);
                }

                return alpha8Data;
            }
        }

        // reruns RGBA8 pixel data from PNG file
        public static byte[] ConvertPngToRGBA8(string texturePath)
        {
            using (var bitmap = new Bitmap(texturePath))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                byte[] textureData = new byte[width * height * 4];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixel = bitmap.GetPixel(x, y);
                        int index = y * width * 4 + x * 4;

                        textureData[index] = pixel.R;
                        textureData[index + 1] = pixel.G;
                        textureData[index + 2] = pixel.B;
                        textureData[index + 3] = pixel.A;
                    }
                }

                return textureData;
            }
        }

        public static Bitmap CreateBitmapFromRawData(byte[] rawData, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(rawData, 0, bmpData.Scan0, rawData.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static void SaveImage(G1TTexture tex, int mipIdx, int layerIdx, string filePath)
        {
            var mip = tex.MipMaps[mipIdx];
            byte[] inputData = mip.Layers[layerIdx];
            int w = (int)mip.Width;
            int h = (int)mip.Height;
            byte[] decoded = DecodeG1t(tex, mipIdx, layerIdx);
            var db = new DirectBitmap<ColorBGRA<byte>, byte>(w, h, decoded);

            if (decoded != null)
            {
                Marshal.Copy(decoded, 0, Marshal.UnsafeAddrOfPinnedArrayElement(db.Bits.ToArray(), 0), db.ByteSize);
                SaveByExtension(db, filePath);
            }

        }

        private static void SaveByExtension<T, TArg>(DirectBitmap<T, TArg> db, string path)
            where TArg : unmanaged
            where T : unmanaged, IColor<TArg>
        {
            string ext = Path.GetExtension(path).ToLower();

            switch (ext)
            {
                case ".png":
                    db.SaveAsPng(path);
                    break;
                case ".jpg":
                case ".jpeg":
                    db.SaveAsJpg(path);
                    break;
                case ".bmp":
                    db.SaveAsBmp(path);
                    break;
                case ".tga":
                    db.SaveAsTga(path);
                    break;
                case ".hdr":
                    db.SaveAsHdr(path);
                    break;
                case ".exr":
                    db.SaveAsExr(path);
                    break;
                case ".dds":
                    db.SaveAsDds(path);
                    break;
                default:
                    db.SaveAsPng(path + ".png");
                    break;
            }
        }

        public static void ConvertDdsToG1T(G1TTexture g1TTexture, string texturesDirPath)
        {
            string[] files = Directory.GetFiles(texturesDirPath, "*.dds", SearchOption.AllDirectories);
            string texPrefix = string.IsNullOrEmpty(g1TTexture.Name) ? "" : g1TTexture.Name;

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!string.IsNullOrEmpty(texPrefix) && !fileName.StartsWith(texPrefix))
                {
                    Console.WriteLine($"Skipping {fileName} - prefix mismatch.");
                    continue;
                }

                string[] parts = fileName.Split('_');
                if (parts.Length < 3)
                {
                    continue;
                }

                int mIdx = -1, lIdx = -1;
                for (int i = parts.Length - 1; i >= 0; i--)
                {
                    if (parts[i].StartsWith("M") && int.TryParse(parts[i].Substring(1), out mIdx)) { }
                    if (parts[i].StartsWith("L") && int.TryParse(parts[i].Substring(1), out lIdx)) { }
                }

                if (mIdx == -1 || lIdx == -1)
                {
                    continue;
                }
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    DdsFile ddsFile = DdsFile.Load(fs);
                    DxgiFormat newDxgiFormat = ddsFile.header.ddsPixelFormat.IsDxt10Format 
                        ? ddsFile.dx10Header.dxgiFormat : ddsFile.header.ddsPixelFormat.DxgiFormat;
                    uint width = ddsFile.header.dwWidth;
                    uint height = ddsFile.header.dwHeight;

                    if (mIdx == 0 && lIdx == 0)
                    {
                        g1TTexture.Width = width;
                        g1TTexture.Height = height;
                        g1TTexture.Format = MapDxgiToG1T(newDxgiFormat);
                    }
                    byte[] ddsData = ddsFile.Faces[0].MipMaps[0].Data;

                    EnsureMipAndLayerExists(g1TTexture, mIdx, lIdx, width, height);

                    g1TTexture.MipMaps[mIdx].Layers[lIdx] = ddsData;
                }

                Console.WriteLine($"Imported: {fileName}");
            }
        }

        private static void EnsureMipAndLayerExists(G1TTexture tex, int mipIdx, int layerIdx, uint w, uint h)
        {
            while (tex.MipMaps.Count <= mipIdx)
            {
                tex.MipMaps.Add(new G1TMipMap { Width = w, Height = h });
            }

            while (tex.MipMaps[mipIdx].Layers.Count <= layerIdx)
            {
                tex.MipMaps[mipIdx].Layers.Add(null);
            }
        }

        private static G1TFormat MapDxgiToG1T(DxgiFormat format)
        {
            return format switch
            {
                DxgiFormat.DxgiFormatBc1Unorm => G1TFormat.BC1_59,
                DxgiFormat.DxgiFormatBc1UnormSrgb => G1TFormat.BC1_59,
                DxgiFormat.DxgiFormatBc2Unorm => G1TFormat.BC2_5A,
                DxgiFormat.DxgiFormatBc3Unorm => G1TFormat.BC3_5B,
                DxgiFormat.DxgiFormatBc4Unorm => G1TFormat.BC4_5C,
                DxgiFormat.DxgiFormatBc5Unorm => G1TFormat.BC5_5D,
                DxgiFormat.DxgiFormatBc7Unorm => G1TFormat.BC7_5F,
                DxgiFormat.DxgiFormatBc7UnormSrgb => G1TFormat.BC7_5F,
                DxgiFormat.DxgiFormatB8G8R8A8Unorm => G1TFormat.B8G8R8A8_0A,
                DxgiFormat.DxgiFormatR8G8B8A8Unorm => G1TFormat.R8G8B8A8_09,
                _ => throw new Exception($"Unsupported DXGI Format for G1T: {format}")
            };
        }

        public static byte[]? DecodeG1t(G1TTexture tex, int mipLevel, int layerIndex = 0)
        {
            if (tex == null || tex.MipMaps.Count == 0)
                return [];

            var mip = tex.MipMaps[mipLevel];

            if (layerIndex >= mip.Layers.Count)
                layerIndex = 0;

            byte[] dataToDecode = mip.Layers[layerIndex];
            int width = (int)mip.Width;
            int height = (int)mip.Height;
            byte[] decodedData = new byte[width * height * 4];
            try
            {
                switch (tex.Format)
                {
                    case G1TFormat.BC1_06:
                    case G1TFormat.BC1_10:
                    case G1TFormat.BC1_59:
                    case G1TFormat.BC1_60:
                        Bc1.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.BC2_07:
                    case G1TFormat.BC2_11:
                    case G1TFormat.BC2_5A:
                    case G1TFormat.BC2_61:
                        Bc2.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.BC3_08:
                    case G1TFormat.BC3_12:
                    case G1TFormat.BC3_5B:
                    case G1TFormat.BC3_62:
                        Bc3.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.BC4_5C:
                    case G1TFormat.BC4_63:
                        Bc4.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.BC5_5D:
                    case G1TFormat.BC5_64:
                        Bc5.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.BC6_5E:
                    case G1TFormat.BC6_65:
                        Bc6h.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, false, decodedData);
                        break;

                    case G1TFormat.BC7_5F:
                    case G1TFormat.BC7_66:
                        Bc7.Decompress<ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.B8G8R8A8:
                    case G1TFormat.B8G8R8A8_0A:
                        RgbConverter.Convert<ColorBGRA<byte>, byte, ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;
                    case G1TFormat.R32G32B32A32_FLOAT:
                        RgbConverter.Convert<ColorRGBA<float>, float, ColorRGBA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;
                    case G1TFormat.R8G8B8A8:

                    case G1TFormat.R8G8B8A8_09:
                    case G1TFormat.RGBA8_UINT_67:
                    case G1TFormat.RGBA8_UINT_74:
                        RgbConverter.Convert<ColorRGBA<byte>, byte, ColorRGBA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.R8_UNORM_2A:
                    case G1TFormat.R8_UNORM_72:
                        RgbConverter.Convert<ColorR<byte>, byte, ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.B5G6R5_UNORM_19:
                        RgbConverter.Convert<ColorRGB16, byte, ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.B5G5R5A1_UNORM_1A:
                        RgbConverter.Convert<ColorRGBA16, byte, ColorBGRA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    case G1TFormat.R16G16B16A16_HALF_FLOAT:
                    case G1TFormat.R16G16B16A16_HALF_FLOAT_0C:
                        RgbConverter.Convert<ColorRGBA<Half>, Half, ColorRGBA<byte>, byte>(dataToDecode, width, height, decodedData);
                        break;

                    default:
                        Console.WriteLine($"Unsupporterd format: {tex.Format}");
                        return null;
                }

                return decodedData;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[G1T Decoder Error]: {ex.Message}");
                return null;
            }
        }

    }
}
