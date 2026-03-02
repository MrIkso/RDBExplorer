using System.Globalization;
using System.Text;

namespace RDBExplorer.Utils
{
    public record RdbTypeInfo
    {
        public string Name { get; init; }
        public string Extension { get; init; }

        public RdbTypeInfo(string name)
        {
            Name = name;
            Extension = string.Empty;
        }

        public RdbTypeInfo(string name, string extension)
        {
            Name = name;
            Extension = extension;
        }
    }

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

        public void UpdateName(uint ktid, string newName, string csvPath = "rdb_names.csv")
        {
            _knownNames[ktid] = newName;
            SaveNamesToCsv(csvPath);
        }

        public void SaveNamesToCsv(string path)
        {
            try
            {
                var lines = _knownNames.Select(kv => $"0x{kv.Key:X8},{kv.Value}");
                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving dictionary: {ex.Message}");
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
                0xAD57EBBA => new("StreamingTexContext", ".g1ts"),
                0x563BDEF1 => new("ModelData", ".g1m"),
                0x786DCD84 => new("G1NFile", ".g1n"),
                0x17614AF5 => new("G1MXFile", ".g1mx"),
                0x6FA91671 => new("G1AFile", ".g1a"),
                0x7BCD279F => new("G1SFile", ".g1s"),
                0x79C724C2 => new("G1PFile", ".g1p"),
                0x54738C76 => new("G1COFile", ".g1co"),
                0xA8D88566 => new("G1COXFile", ".g1cox"),
                0x7461C7CA => new("G1HFile", ".g1h"), // ShapeData
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

                // database types
                0xE2715252 => new("Sprite"),
                0xE0D88E3F => new("Code"),
                0xC27BF870 => new("FCurve"),
                0xC4B9B28D => new("ClothCollision"),
                0xF7C0D0F0 => new("TexPatternAnimData"),
                0x22A5E641 => new("CharacterSettingLoader"),
                0x79991E79 => new("StaticScreenLayoutTextures"),
                0x9D1AFA46 => new("ScreenLayoutTexturePackSettings"),
                0xBBB90D65 => new("SpriteText"),
                0xE3C72C83 => new("TextData"),
                0x72233E17 => new("Plane"),
                0x6AF08512 => new("ScreenLayoutBlurPane"),
                0xF1FBFFBA => new("Empty"),
                0x9E20A442 => new("SpriteWindow"),
                0x23E7C64E => new("Ortho"),
                0xBB5CC587 => new("ResourceKeeper"),
                0x918C505C => new("MotorEventSetting"),
                0x04C49E64 => new("Decal"),
                0x1CA58BA0 => new("MotorStageVisibilitySet"),
                0x5D4A99D6 => new("Scattering"),
                0xCB173E65 => new("Distance"),
                0x9773131B => new("ThreePoints"),
                0x73B95A31 => new("SkylightDependingParams"),
                0xE01606F4 => new("Point"),
                0x13696D79 => new("System"),
                0x2EB79141 => new("Array"),
                0x0AB9B5DD => new("Point"),
                0x2D4D9F2F => new("Spot"),
                0x63026395 => new("Folder"),
                0xF041CE62 => new("Location"),
                0xDDD0E775 => new("OccluderUnit"),
                0x1A83479F => new("ProceduralPlacement"),
                0x3BC0342D => new("StreamWaterRiver"),
                0xE736B039 => new("WorldPQ"),
                0x34F2EC8B => new("G1A"),
                0x3B6F1DAB => new("G1A"),
                0xB57B488B => new("OboroStaticReflectionProbeField"),
                0x8E1ECB72 => new("Landscape"),
                0xC388EA58 => new("LandscapePalette"),
                0x3059B9C3 => new("Textures"),
                0xFF7DBFD4 => new("Static"),
                0xEC5647E9 => new("Effect"),
                0x11C2F858 => new("OboroStaticReflectionProbeBlock"),
                0xD051FAD8 => new("OboroStaticReflectionProbeFieldManager"),
                0x9DCFCCE8 => new("OboroStaticReflectionProbeFieldEntity"),
                0x4670A0F1 => new("NonResident"),
                0x6C01215E => new("ModelRenderPartsSetTypeContainer"),
                0xA880E514 => new("PartsSet"),
                0x0E3EFB53 => new("Sctipt"),
                0x071D73F1 => new("PathFinderObject"),
                0x077CABC9 => new("NavMapDataObject"),
                0xA1A84F2C => new("MotorScreenLayoutSetting"),
                0x63336873 => new("StaticScreenLayout"),
                0x9C1CED03 => new("FontParameter"),
                0x8361F814 => new("OffScreenRenderingParams"),
                0x215594C2 => new("StaticScreenLayoutViewMask"),
                0x1081C881 => new("StaticScreenLayoutScrollView"),
                0xF3DDB690 => new("StaticScreenLayoutListView"),
                0xBC112422 => new("StaticScreenLayoutBase"),
                0x8D034D41 => new("MotorScreenLayoutCommonSetting"),
                0xDE4B32E8 => new("MotorFilePathBased"),
                0xAE2BF5DD => new("MotorFilePathBasedFixModel"),
                0x7AC0C15A => new("MotorAISetting"),
                0xE2E5D9A1 => new("MotorWeatheringBake"),
                0x389FFFF6 => new("MotorMemoryAddressStatic"),
                0xDCD18E09 => new("MotorFilePathBasedStatic"),
                0x889C29D3 => new("MotorFigureTriangleStripBoxListManager"),
                0x48AC71E2 => new("MotorFigureTriangleStripPlaneListManager"),
                0xBC6B84B0 => new("MotorCharacterCreationFactory"),
                0x38BAB111 => new("MotorCharacterCreationModifier"),
                0x849AA5BE => new("MotorLineList"),
                0x57DCFBC1 => new("MotorSequenceSetting"),
                0xE1D40600 => new("MotorStageSetting"),
                0xDAC911D7 => new("MotorCommonSetting"),
                0x71555F6A => new("MotorKtidFilePathBasedTextures"),
                0x82A3F186 => new("MotorFilePathBasedTextures"),
                0xD186CEEB => new("MotorCharacterSetting"),
                0xD40B3C8F => new("Model"),
                0x5B50E299 => new("IES"),
                0xD8004A77 => new("Sound"),
                0x11A85121 => new("SequentialPlayer"),
                0x0FE035D1 => new("Model"),
                0xED0038D4 => new("MotionSoundInvalidator"),
                0xCAFC88F2 => new("ApplyAnimation"),
                0x7BDD07F3 => new("BlendPlayer"),
                0xADC35BA3 => new("Camera"),
                0xC72F2CBE => new("SpotWind"),
                0xD3400677 => new("AnimationGraph"),
                0x5A22481C => new("Bank"),
                0x6310F3F9 => new("BlendSpace1D"),
                0xD88B1A3A => new("BlendSpace2D"),
                0x8A69EF4E => new("RadialBlurParams"),
                0x779E7755 => new("MotionMatchingDatabase"),
                0xF4C31252 => new("RenderParts"),
                0x675CE194 => new("FullSpecForwardRenderingParams"),
                0xC40552A0 => new("FontsetWriterParameter"),
                0xAB9C4087 => new("PhysicallyBased2Tree2"),
                0xCFF247A4 => new("PhysicallyBased2Anisotropic"),
                0xF71448B6 => new("PhysicallyBased2Fur"),
                0x39E93192 => new("Shadowmap"),
                0x4245A167 => new("ShadowmapCube"),
                0x1A62DB48 => new("PhysicallyBasedTRFRMetal"),
                0x6794EA8A => new("ShadowmapGeneral"),
                0x840460B9 => new("Standart"),
                0xBA7900F0 => new("PhysicallyBasedTRFRBoth"),
                0xF97D6F4A => new("PhysicallyBasedLandscape"),
                0x3932EF6E => new("PhysicallyBased2Landscape"),
                0x1E370F3F => new("PhysicallyBased2DeferredMiasmaDecal"),
                0xC78FE503 => new("PhysicallyBased2ScreenMap"),
                0x9BCC2D0D => new("UltraMarineBakedArea"),
                0xBD97D9BA => new("UltraMarineIntegrateWave"),
                0x04583254 => new("PhysicallyBased2CausticEye"),
                0x4C182E35 => new("Reflection"),
                0xE782017D => new("UltraMarineWave"),
                0x6EBC1F4A => new("UltraMarine"),
                0xD8E4C49A => new("UltraMarineBakeAmplitude"),
                0x5E061A21 => new("MeshletGeometryDensityDebug"),
                0x0A8684A2 => new("ARPSSS"),
                0x23E223FB => new("UltraMarineProfile"),
                0xADC08BB6 => new("UltraMarineBakeWave"),
                0x26BAA8F1 => new("MeshletPreculling"),
                0xC5AB5B5A => new("MeshletCheckLoadRequestCommand"),
                0xB94E4C61 => new("UltraMarineAmplitude"),
                0xBA7A9C71 => new("PhysicallyBased2MultiwayLighting"),
                0x5BECC728 => new("UltraMarineFloat"),
                0x78D2CC1E => new("ReflectionProbeSetup"),
                0xDAFD744A => new("Resolve4KCheckerboard"),
                0x4F60BD65 => new("WriteDirectOcclusionSST"),
                0x3EBA1E39 => new("MeshletResolveLOD"),
                0xAF46AFDB => new("MeshletGenerateLoadRequestCommand"),
                0x946F730E => new("UltraMarineIgnite"),
                0xBDA17D38 => new("River2TexBufferWrite"),
                0xBE04856A => new("UltraMarineTopView"),
                0xF0166925 => new("UltraMarineCaustics"),
                0x6E43173B => new("UltraMarineModel"),
                0x157D5A62 => new("ComputeVertexSkinning"),
                0x752EBA0E => new("BuildProbeField"),
                0xF637C448 => new("SnowBlurScattering"),
                0xDED04CB8 => new("ShadowTree"),
                0x99E5F965 => new("VirtualShadowMap"),
                0xB6E8F301 => new("RHMUtility"),
                0x0FB246DB => new("ReflectionProbe"),
                0xCD566BC9 => new("MeshletInstanceCulling"),
                0x4D51241E => new("UltraMarineIntegrateProperty"),
                0x8DE94ADE => new("UltraMarineWet"),
                0x8573A359 => new("UltraMarineDynamicMask"),
                0xA08C9AD0 => new("UltraMarineBubble"),
                0x777247FC => new("MeshletGenerateHiZMap"),
                0xB53ADB58 => new("LightOuterTexture"),
                0x8B599252 => new("Instancing"),
                0xF9D47741 => new("InstancingModelManager"),
                0x60A5ABFF => new("G1FRAni", ".g1fr"),
                0xBA3C84BD => new("PoseBlendAnimation"),
                0x8725D306 => new("G1FPose", ".g1fpose"),
                0xF57106C4 => new("SoundEmitterTile"),
                0x87D3FEEF => new("Capsule"),
                0x1180647B => new("UltraMarineDynamicMaskObject"),
                0xE6FB4493 => new("Box"),
                0x2A39F891 => new("River2"),
                0xADE35F4A => new("SoundEmitterVoxel"),
                0xE6379BA2 => new("ReflectionProbeField"),
                0x13797068 => new("BlendShape"),
                0x070EAECD => new("TwistGenerator"),
                0x0C3974B8 => new("ShareablePlane"),
                0x977C5F0F => new("VideoMemoryKeeper"),
                0x534B33F2 => new("StreamingCube"),
                0xEB6DA0E1 => new("StreamingPlane"),
                0x8687A049 => new("HighlightPass2"),
                0xEB1BE016 => new("CombinedArray"),
                0x2EEA1225 => new("ForBake"),

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