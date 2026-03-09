using RDBExplorer.Utils;
using static RDBExplorer.Core.Formats.KSCL.KSCLFile;

namespace RDBExplorer.Core.Formats.KSCL
{
    public class KSCLParser
    {
        public KSCLFile GetKSCLFile { get; set; }

        public void Parse(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BinaryReader(ms))
            {
                var kscl = new KSCLFile();
                kscl.Header = reader.ReadStruct<KSCLHeader>();

                char[] magicArray = kscl.Header.Magic.Reverse().ToArray();
                string magicStr = new string(magicArray);
                if (magicStr != "KSCL")
                    throw new Exception($"Not a KSCL file. Found: {magicStr}");

                for (int i = 0; i < kscl.Header.SectionCount; i++)
                {
                    var secHeader = reader.ReadStruct<SectionHeader>();
                    long currentPos = reader.BaseStream.Position;
                    var section = new Section { Info = secHeader };
                    section.DictionaryData = reader.ReadBytes((int)secHeader.DictionarySize);
                    section.SectionData = reader.ReadBytes((int)secHeader.SecionTableSize);
                    /*if (secHeader.ItemCount > 0 && secHeader.DictionarySize > 0)
                    {
                          ParseDictionary(section);
                    }*/

                    kscl.Sections.Add(section);
                }

                GetKSCLFile = kscl;
            }
        }
    }
}
