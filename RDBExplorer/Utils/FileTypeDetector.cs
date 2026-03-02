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
            new Signature("? 32 30 31 31 2D 30 34 2D 32 39 54 31 34 3A 32 36 3A 34 38 2B 30 39 30 30", ".bytecode"), // Game comiled scripts
            new Signature("00 22 04 20", ".database"), // might be game databases
            //AddAsciiSignature("DXBC", ".dxbc"),       // unknown
            AddAsciiSignature("_A2G", ".g1a"),        // G1AFile
            AddAsciiSignature("OC1G", ".g1co"),       // Collision data
            AddAsciiSignature("ME1G", ".g1em"),       // EffectMeshData
            AddAsciiSignature("SE1G", ".g1es"),       // EffectShapeMeshData
            AddAsciiSignature("XF1G", ".g1e"),        // EffectData
            AddAsciiSignature("_H1G", ".g1h"),        // G1HFile
            AddAsciiSignature("II1G", ".gii"),        // G1IIFile
            AddAsciiSignature("_M1G", ".g1m"),        // ModelData
            //AddAsciiSignature("MN1G", ".g1nm"),      // unknown
            AddAsciiSignature("_N1G", ".g1n"),        // G1NFile
            //AddAsciiSignature("FP1G", ".g1pf"),      // unknown
            AddAsciiSignature("2R1G", ".rbg"),        // River2BakedGeometry
            AddAsciiSignature("GT1G", ".g1t"),        // TexContext / StreamingTexContext
            AddAsciiSignature("_A2G", ".g1a"),        // G1AFile (було .g2a)
            AddAsciiSignature("_S2G", ".g1s"),        // G1SFile (було .g2s)
            AddAsciiSignature("FBRG", ".grbf"),       // RBFData
            //AddAsciiSignature("_TGK", ".kgt"),        // unknown
            AddAsciiSignature("_RGK", ".kidsrender"), // RenderGraphFile
            //AddAsciiSignature("_MHK", ".khm"),        // unknown
            AddAsciiSignature("_DOK", ".kidsobjdb"),  // ObjectDatabaseFile
            AddAsciiSignature("LCSK", ".kscl"),       // KSCLFile
            AddAsciiSignature("GSTK", ".kts"),        // TexStageTableBinaryFile
            AddAsciiSignature("1FPO", ".g1fpose"),    // FPoseData
            AddAsciiSignature("BGIR", ".rigbin"),     // RigBinFile
            //AddAsciiSignature("_DVS", ".svd"),        // unknown
            AddAsciiSignature("CGRS", ".sgcbin"),     // GlobalConfiguration 
            AddAsciiSignature("ASRS", ".srsa"),       // AssetData
            AddAsciiSignature("TSRS", ".srst"),       // StreamAssetDataFile
            AddAsciiSignature("XC1G", ".g1cox"),      // G1COXFile
            AddAsciiSignature("1FRA", ".g1frani"),    // FRAnimationData
            AddAsciiSignature("M1OK", ".g1mx"),       // G1MXFile
            AddAsciiSignature("P1GK", ".g1p"),        // G1PFile
            AddAsciiSignature("mmdb", ".mmdb"),       // MotionMatchingDatabase
            AddAsciiSignature("SWGQ", ".swg"),        // SwingData

            AddAsciiSignature("G2A_PACK", ".g2apack"), // animation pack
            AddAsciiSignature("TMG_PACK", ".tmgpack"),
            AddAsciiSignature("TRMD", ".dmrt"),
            AddAsciiSignature("DMPP", ".dmpp"),
        };

        public static string DetectExtension(byte[] data)
        {
            if (data == null || data.Length < 4)
                return ".dat";

            ReadOnlySpan<byte> header;
            if (data.Length > 32)
            {
                header = data.AsSpan(0, 32);
            }
            else if (data.Length > 16)
            {
                header = data.AsSpan(0, 16);
            }
            else
            {
                header = data.AsSpan();
            }
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
