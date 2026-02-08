using System.Globalization;
using System.Text;

namespace RDBExplorer.Utils
{
    public record RdbTypeInfo(string Name, string Extension);

    public class TypeIDHelper
    {
        private static readonly Lazy<TypeIDHelper> _instance = new(() => new TypeIDHelper());
        public static TypeIDHelper Instance => _instance.Value;

        // Key = FileKtid, Value = Real Name
        private readonly Dictionary<uint, string> _knownNames = new();

        private TypeIDHelper() { }

        /// <summary>
        /// Loads names from a CSV file. Format: 0xHash,Name
        /// </summary>
        public void LoadNamesFromCsv(string path)
        {
            if (!File.Exists(path))
                return;

            _knownNames.Clear();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 2) continue;

                string hexHash = parts[0].Trim().Replace("0x", "");
                if (uint.TryParse(hexHash, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint ktid))
                {
                    _knownNames[ktid] = parts[1].Trim();
                }
            }
        }

        public string GetFileName(uint fileKtid, uint typeInfoKtid)
        {
            if (_knownNames.TryGetValue(fileKtid, out string name))
            {
                return name;
            }
            return $"0x{fileKtid:X8}{GetExtension(typeInfoKtid)}";
        }

        public static RdbTypeInfo GetInfo(uint typeId)
        {
            return typeId switch
            {
                // common files
                0xAFBEC60C => new("TexContext", ".g1t"),
                0xAD57EBBA => new("StreamingTexContext", ".g1t"),
                0x563BDEF1 => new("ModelData", ".g1m"),
                0x786DCD84 => new("G1NFile", ".g1n"),
                0x17614AF5 => new("G1MXFile", ".g1mx"),
                0x6FA91671 => new("G1AFile", ".g1a"),
                0x7BCD279F => new("G1SFile", ".g1s"),
                0x79C724C2 => new("G1PFile", ".g1p"),
                0x54738C76 => new("G1COFile", ".g1co"),
                0xA8D88566 => new("G1COXFile", ".g1cox"),
                0x7461C7CA => new("G1HFile", ".g1h"),
                0xDB0AE0AA => new("G1IIFile", ".gii"),
                0xB097D41F => new("EffectData", ".g1e"),
                0x4D0102AC => new("EffectMeshData", ".g1em"),
                0x1A6300FD => new("EffectShapeMeshData", ".g1es"),
                0x2BCC0C02 => new("FRAnimationData", ".g1frani"),
                0x32AC9403 => new("FPoseData", ".g1fpose"),

                // system db
                0x20A6A0BB => new("ObjectDatabaseFile", ".kidsobjdb"),
                0xBF6B52C7 => new("NameDatabaseFile", ".name"),
                0x1FDCAA40 => new("TaskGraphFile", ".kidstask"),
                0xB1630F51 => new("RenderGraphFile", ".kidsrender"),
                0xBE144B78 => new("KTIDFile", ".ktid"),
                0x8E39AA37 => new("KTIDFileBinary", ".ktid"),
                0xB0A14534 => new("GlobalConfiguration", ".sgcbin"),
                0x8D735C52 => new("OBOROStaticResourceBinaryFile", ".oboro"),
                // bingings and tables
                0x1AB40AE8 => new("OIDBindTableBinaryFile", ".oid"),
                0xDBCB74A9 => new("OIDFile", ".oid"),
                0xE6A3C3BB => new("OIDBindTableBinaryFileEx", ".oidex"),
                0x9CB3A4B6 => new("OIDExFile", ".oidex"),
                0x753AA042 => new("OIDSQTBindTableBinaryFile", ".oidsq"),

                0xB340861A => new("MaterialGroupBindTableBinaryFile", ".mtl"),
                0x56EFE45C => new("PartsModelGroupBindTableBinaryFile", ".grp"),
                0xBBF9B49D => new("GroupFile", ".grp"),
                0x27BC54B7 => new("RigBinFile", ".rigbin"),

                // scripts and coalisions
                0x5599AA51 => new("KSCLFile", ".kscl"),
                0x4F16D0EF => new("KTSFile", ".kts"),
                0xED410290 => new("TexStageTableBinaryFile", ".kts"),

                // ui and text
                0xA1BDB205 => new("G2NFile", ".g2n"),
                0x96C74B4F => new("G2NGlyphSetFile", ".g2n"),
                0xC9D883C2 => new("ScreenLayoutColorTableBinaryFile", ".colortable"),
                0xF13845EF => new("ScreenLayoutShapeInfoFile", ".sclshape"),
                0xF20DE437 => new("StaticScreenLayoutTexInfoFile", ".texinfo"),

                // audio and video
                0xBBD39F2D => new("AssetData", ".srsa"),
                0x0D34474D => new("StreamAssetDataFile", ".srst"),
                0x133D2C3B => new("ShaderBindTableBinaryFile", ".sid"),
                0xA027E46B => new("VideoStreamset", ".mov"),
                0xBEF563DD => new("StreamingMeshletModelData", ".g1m"),

                0x5B2970FC => new("KTF2File", ".ktf2"),
                0xD7F47FB1 => new("BinaryFile", ".efpl"),
                0x193D2E44 => new("RBFData", ".grbf"),
                0x4638B72D => new("River2BakedGeometry", ".rbg"),
                0x5C3E543C => new("SwingData", ".swg"),
                0x82945A44 => new("LandscapeQuadtree", ".lsqtree"),
                0xCBFD49B2 => new("MotionMatchingDatabase", ".mmdb"),
                0x0BD05B27 => new("MITFile", ".mit"),
                0x6DBD6EA6 => new("CSVFile", ".mit"),
                0xF02F31AB => new("OIDBindTable", ""),
                _ => new($"Unknown_0x{typeId:X8}", "")
            };
        }

        public static string GetTypeName(uint typeId) => GetInfo(typeId).Name;
        public static string GetExtension(uint typeId) => GetInfo(typeId).Extension;

        private static byte[] ToBytes(uint magic)
        {
            byte[] bytes = BitConverter.GetBytes(magic);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

    }
}
