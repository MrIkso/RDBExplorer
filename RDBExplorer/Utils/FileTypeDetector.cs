using System.Globalization;
using System.Text;

namespace RDBExplorer.Utils
{
    public class Signature
    {
        public byte[] Pattern { get; }
        public bool[] Mask { get; }
        public string Extension { get; }

        public Signature(string hexMask, string extension)
        {
            Extension = extension;
            string[] parts = hexMask.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Pattern = new byte[parts.Length];
            Mask = new bool[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "??" || parts[i] == "?")
                {
                    Pattern[i] = 0x00;
                    Mask[i] = false;
                }
                else
                {
                    Pattern[i] = byte.Parse(parts[i], NumberStyles.HexNumber);
                    Mask[i] = true;
                }
            }
        }

        public bool IsMatch(ReadOnlySpan<byte> data)
        {
            if (data.Length < Pattern.Length) return false;

            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Mask[i] && data[i] != Pattern[i])
                    return false;
            }
            return true;
        }
    }

    public static class FileTypeDetector
    {
        private static readonly List<Signature> Signatures = new List<Signature>
        {
            new Signature("? 50 4E 47", ".png"),
            new Signature("? 32 30 31 31 2D 30 34 2D 32 39 54 31 34 3A 32 36 3A 34 38 2B 30 39 30 30", ".bytecode"),
            AddAsciiSignature("DXBC", ".dxbc"),
            AddAsciiSignature("_A1G", ".g1a"),
            AddAsciiSignature("OC1G", ".g1co"),
            AddAsciiSignature("ME1G", ".g1em"),
            AddAsciiSignature("SE1G", ".g1es"),
            AddAsciiSignature("XF1G", ".g1fx"),
            AddAsciiSignature("_H1G", ".g1h"),
            AddAsciiSignature("II1G", ".g1ii"),
            AddAsciiSignature("_M1G", ".g1m"),
            AddAsciiSignature("MN1G", ".g1nm"),
            AddAsciiSignature("_N1G", ".g1n"),
            AddAsciiSignature("FP1G", ".g1pf"),
            AddAsciiSignature("2R1G", ".g1r2"),
            AddAsciiSignature("GT1G", ".g1t"),
            AddAsciiSignature("_A2G", ".g2a"),
            AddAsciiSignature("_S2G", ".g2s"),
            AddAsciiSignature("FBRG", ".grbf"),
            AddAsciiSignature("_TGK", ".kgt"),
            AddAsciiSignature("_RGK", ".kgr"),
            AddAsciiSignature("_MHK", ".khm"),
            AddAsciiSignature("_DOK", ".kod"),
            AddAsciiSignature("LCSK", ".kscl"),
            AddAsciiSignature("GSTK", ".ktsg"),
            AddAsciiSignature("1FPO", ".opf1"),
            AddAsciiSignature("BGIR", ".rigb"),
            AddAsciiSignature("_DVS", ".svd"),
            AddAsciiSignature("CGRS", ".srgc"),
            AddAsciiSignature("ASRS", ".srsa"),
            AddAsciiSignature("TSRS", ".srst"),

            AddAsciiSignature("G2A_PACK", ".g2apack"),
            AddAsciiSignature("TMG_PACK", ".tmgpack"),
            AddAsciiSignature("TRMD", ".dmrt"),
            AddAsciiSignature("DMPP", ".dmpp"),
        };

        public static string DetectExtension(byte[] data)
        {
            if (data == null || data.Length < 4)
                return ".dat";

            ReadOnlySpan<byte> header = data.Length > 16 ? data.AsSpan(0, 32) : data.AsSpan();

            foreach (var sig in Signatures)
            {
                if (sig.IsMatch(header))
                {
                    return sig.Extension;
                }
            }

            return ".dat";
        }

        public static Signature AddAsciiSignature(string ascii, string ext)
        {
            string hex = string.Join(" ", Encoding.ASCII.GetBytes(ascii).Select(b => b.ToString("X2")));
            return new Signature(hex, ext);
        }
    }
}
