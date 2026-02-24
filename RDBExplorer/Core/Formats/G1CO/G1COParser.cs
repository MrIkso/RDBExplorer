using RDBExplorer.Utils;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RDBExplorer.Core.Formats.G1CO
{
    public class G1COParser
    {
        public G1COFile GetCOFile { get; set; }

        public void Parse(byte[] data)
        {
            GetCOFile = new G1COFile();
            G1COHeader header;
            List<HVB> HVBs = new List<HVB>();

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms, Encoding.ASCII))
            {
                header = new G1COHeader
                {
                    Magic = br.ReadEncodedString(4),
                    Version = br.ReadEncodedString(4),
                    Unk = br.ReadByte(),
                    MetadataSize = br.ReadByte(),
                    NumEntries = br.ReadByte(),
                    DispatchMode = br.ReadByte(),
                    FileSize = br.ReadUInt32()
                };

                if (header.Magic != "OC1G")
                    throw new Exception("Invalig G1CO magic");

                if (header.MetadataSize > 0)
                {
                    br.BaseStream.Position += header.MetadataSize;
                }

                for (int i = 0; i < header.NumEntries; i++)
                {
                    HVB hvb = new();
                    hvb.Read(br);
                    HVBs.Add(hvb);
                }

                GetCOFile.Header = header;
                GetCOFile.HVBs = HVBs;
            }
        }
    }

   
}
