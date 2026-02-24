using RDBExplorer.Utils.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Formats.OID
{
    public class OIDEntry
    {
        public uint Index { get; set; }

        [JsonConverter(typeof(JsonHexUintConverter))]
        public uint KtidHash { get; set; }
        public uint Reverserd { get; set; }
    }

    public class OIDFile
    {
        public uint Unk { get; set; }
        public List<OIDEntry> Entries { get; set; } = new List<OIDEntry>();
    }

    public class OIDParser
    {
        public OIDFile OIDFile { get; set; }

        public void Load(byte[] data)
        {
            OIDFile = new OIDFile();
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {

                OIDFile.Unk = br.ReadUInt32();
                
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    OIDFile.Entries.Add(new OIDEntry
                    {
                        Index = br.ReadUInt32(),
                        KtidHash = br.ReadUInt32(),
                        Reverserd = br.ReadUInt32(),
                    });
                }
            }
        }
    }
}
