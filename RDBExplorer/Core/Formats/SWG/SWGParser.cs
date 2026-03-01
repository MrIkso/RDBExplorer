using RDBExplorer.Core.Formats.GRBF;
using RDBExplorer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Formats.SWG
{
    public class SWGParser
    {
        public SWGFile File { get; set; }

        public void Load(byte[] data)
        {
            File = new SWGFile();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                SWGHeader header = new SWGHeader();
                header.Magic = br.ReadEncodedString(4);
                header.BlockSize = br.ReadUInt32();
                header.EntryCount = br.ReadUInt32();
                header.GroupMame = br.ReadEncodedString(8);
                header.Unk1 = br.ReadUInt32();
                header.Unk2 = br.ReadUInt32();
                header.Unk3 = br.ReadUInt32();

                File.Header = header;

                List<SWGEntry> entries = new List<SWGEntry>();
                for (int i = 0; i < header.EntryCount; i++)
                {
                    SWGEntry entry = br.ReadStruct<SWGEntry>();
                    File.Entries.Add(entry);
                }
            }
        }
    }
}
