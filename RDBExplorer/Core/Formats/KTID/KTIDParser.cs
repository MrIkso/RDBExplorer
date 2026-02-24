using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.KTID
{
    public class KTIDEntry
    {
        public uint Index { get; set; }
        public uint KtidHash { get; set; }
    }

    public class KTIDParser
    {
        public List<KTIDEntry> Entries { get; set; } = new List<KTIDEntry>();

        public void Load(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                Entries.Clear();
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    Entries.Add(new KTIDEntry
                    {
                        Index = br.ReadUInt32(),
                        KtidHash = br.ReadUInt32()
                    });
                }
            }
        }
    }
}
