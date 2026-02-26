using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Formats.GRBF
{
    public class GRBFEntry
    {
        public uint BlockSize { get; set; }
        public uint[] Data { get; set; }
    };

    public class GRBFHeader
    {
        public string Magic { get; set; }
        public string Version { get; set; }
        public uint BlockSize { get; set; }
        public uint EntriesCount { get; set; }

    }
    public class GRBFFile
    {
        public GRBFHeader Header { get; set; }
        public List<GRBFEntry> Entries { get; set; } = new List<GRBFEntry>();
    }
}
