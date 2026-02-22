using RDBExplorer.Core.Models;
using System.ComponentModel;

namespace RDBExplorer.Core.G1T
{
    public enum G1TLoadType : byte
    {
        PLANAR = 0,
        CUBE = 1, // 6 sided
        VOLUME = 2, // AKA 3D
        PLANE_ARRAY = 3, // AKA 2D array
        CUBE_ARRAY = 4  // ver 63 and above
    }

    public enum G1TFormat : byte
    {
        // Uncompressed
        R8G8B8A8 = 0x00,
        B8G8R8A8 = 0x01,
        R32_FLOAT = 0x02,
        R16G16B16A16_HALF_FLOAT = 0x03,
        R32G32B32A32_FLOAT = 0x04,
        D24S8 = 0x05,

        BC1_06 = 0x06, // DXT1
        BC2_07 = 0x07, // DXT2/3
        BC3_08 = 0x08, // DXT4/5

        R8G8B8A8_09 = 0x09,
        B8G8R8A8_0A = 0x0A,
        R32_FLOAT_0B = 0x0B,
        R16G16B16A16_HALF_FLOAT_0C = 0x0C,
        RGBA32_FLOAT_0D = 0x0D,
        A2B10G10R10_HALF_FLOAT = 0x0E,
        A8_UNORM_0F = 0x0F,
        
        BC1_10 = 0x10,
        BC2_11 = 0x11,
        BC3_12 = 0x12,
        D24S8_13 = 0x13,

        D16 = 0x14,
        A8_UNORM_18 = 0x18,
        B5G6R5_UNORM_19 = 0x19,
        B5G5R5A1_UNORM_1A = 0x1A,

        R8_UNORM_2A = 0x2A,

        R10G10B10A2_UNORM_40 = 0x40,
        RGBA16_UNORM_41 = 0x41,
        R8G8_UNORM_46 = 0x46,
        RG32_FLOAT_4C = 0x4C,
        R32_FLOAT_4E = 0x4E,

        BC1_59 = 0x59, // DXT1 (Modern)
        BC2_5A = 0x5A, // DXT3 (Modern)
        BC3_5B = 0x5B, // DXT5 (Modern)
        BC4_5C = 0x5C, // ATI1
        BC5_5D = 0x5D, // ATI2 / BC5
        BC6_5E = 0x5E, // BC6H (HDR)
        BC7_5F = 0x5F, // BC7 (High Quality)

        BC1_60 = 0x60,
        BC2_61 = 0x61,
        BC3_62 = 0x62,
        BC4_63 = 0x63,
        BC5_64 = 0x64,
        BC6_65 = 0x65,
        BC7_66 = 0x66,

        RGBA8_UINT_67 = 0x67,
        RG8_UINT_68 = 0x68,
        RG16_FLOAT_69 = 0x69,
        R16_FLOAT_6A = 0x6A,
        R11G11B10_FLOAT_6B = 0x6B,

        R8_UNORM_72 = 0x72,
        RG8_UNORM_73 = 0x73,
        RGBA8_UINT_74 = 0x74,
        RG8_UINT_75 = 0x75,
        RG16_FLOAT_76 = 0x76,
        R16_FLOAT_77 = 0x77,
        R11G11B10_FLOAT_78 = 0x78
    }

    public enum EX_SWIZZLE_TYPE: byte
    {
        NONE = 0,
        DX12_64kb       = 0x01,
        ZLIB_COMPRESSED = 0x03, // new to v66
    };


[TypeConverter(typeof(ExpandableObjectConverter))]
    public class G1THeader
    {
        public uint Magic { get; set; } = 0x47315447; // "GT1G"
        public uint Version { get; set; } // "0060"
        public uint FileSize { get; set; }
        public uint TableOffset { get; set; }
        public uint NumTextures { get; set; }
        public KoeiPlatform Platform { get; set; }
        public uint MetadataSize { get; set; }
        public uint Unk1C { get; set; }
        public byte[] GlobalMetadata { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class G1TMipMap
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public List<byte[]> Layers { get; set; } = new List<byte[]>();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class G1TTexture
    {
        public G1TLoadType LoadType { get; set; }
        public G1TFormat Format { get; set; }
        public EX_SWIZZLE_TYPE EX_SwizzleType { get; set; } = EX_SWIZZLE_TYPE.NONE;
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint ZScale { get; set; }

        public int MipCount { get; set; }
        public uint ExFaces { get; set; }
        public uint Depth { get; set; }
        public byte[] Metadata { get; set; }
        public byte ExtraHeaderVersion { get; set; }
        public uint ExtraHeaderSize { get; set; }
        public uint ArraySize { get; set; } 
        public byte[] ExtraHeaderRaw { get; set; }
        public uint NormalMapFlags { get; set; }
        public List<G1TMipMap> MipMaps { get; set; } = new List<G1TMipMap>();

        public string Name { get; set; } = string.Empty;

        public uint GetTotalLayers()
        {
            uint baseLayers = Math.Max(1, ArraySize);
            if (LoadType == G1TLoadType.CUBE || LoadType == G1TLoadType.CUBE_ARRAY)
            {
                return baseLayers * 6;
            }
            return baseLayers;
        }

        public byte GetDxDyByte()
        {
            int wLog = (int)Math.Ceiling(Math.Log(Width, 2));
            int hLog = (int)Math.Ceiling(Math.Log(Height, 2));
            return (byte)(hLog << 4 | wLog & 0x0F);
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class G1TFile
    {
        public G1THeader Header { get; set; } = new G1THeader();
        public List<G1TTexture> Textures { get; set; } = new List<G1TTexture>();

    }
}