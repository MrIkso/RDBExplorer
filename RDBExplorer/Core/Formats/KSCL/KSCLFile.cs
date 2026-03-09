using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Formats.KSCL
{
    public class KSCLFile
    {
        public KSCLHeader Header;
        public List<Section> Sections = new List<Section>();

        public class Section
        {
            public SectionHeader Info;
            public byte[] DictionaryData;
            public byte[] SectionData;
            //public List<DictionaryItem> Dictionary = new List<DictionaryItem>();
        }

        public struct DictionaryItem
        {
            public uint Id;
            public byte[] Payload;
        }
    }
}
