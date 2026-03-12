using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Formats.G1M
{
    public class G1MGPropertySet
    {
        public List<G1MGProperty> Properties = new();
    }

    public class G1MGProperty
    {
        public string Name { get; set; }
        public ushort Type { get; set; }
        public ushort UnkFlag1 { get; set; }
        public ushort UnkFlag2 { get; set; }
        public ushort UnkFlag3 { get; set; }
        public byte[] Data { get; set; }

        public ushort SpecialFlag { get; set; }
    }
}
