using RDBExplorer.Utils;

namespace RDBExplorer.Core.Formats.TexInfo
{
    public struct TextInfoParam
    {
        public uint AtlasIndex { get; set; }
        public float UV_TopLeft_U { get; set; }
        public float UV_TopLeft_V { get; set; }
        public float UV_TopRight_U { get; set; }
        public float UV_TopRight_V { get; set; }
        public float UV_BottomLeft_U { get; set; }
        public float UV_BottomLeft_V { get; set; }
        public float UV_BottomRight_U { get; set; }
        public float UV_BottomRight_V { get; set; }
        public uint UnknownInt2 { get; set; }
    }

    public class TextInfoEntry
    {
        public string TextureName {  get; set; }

        public TextInfoParam TextInfoParam { get; set; }

    }

    public class TexInfoFile
    {
        public uint EntriesCount { get; set; }
        public List<TextInfoEntry> Entries { get; set; } = new List<TextInfoEntry>();

    }
    public class TextInfoParser
    {
        public TexInfoFile TexInfoFile { get; set; } 
        public void Load(byte[] data)
        {
            TexInfoFile = new TexInfoFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                TexInfoFile.EntriesCount = br.ReadUInt32();

                for (int i = 0; i < TexInfoFile.EntriesCount; i++)
                {
                    string name = br.ReadEncodedString(64);
                    var param = br.ReadStruct<TextInfoParam>();

                    TextInfoEntry textInfoEntry = new TextInfoEntry();
                    textInfoEntry.TextureName = name;
                    textInfoEntry.TextInfoParam = param;
                    TexInfoFile.Entries.Add(textInfoEntry);
                }
            }
        }
    }
}
