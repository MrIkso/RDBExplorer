using System.Text;

namespace RDBExplorer.Core.G1T
{
    public class G1tReader
    {
        public G1tHeader Header { get; private set; }
        public List<G1tTexture> Textures { get; private set; }

        public void Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Load(fs);
            }
        }

        public void Load(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                Load(ms);
            }
        }

        public void Load(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true))
            {
                Read(br);
            }
        }

        private void Read(BinaryReader br)
        {
            long startPos = br.BaseStream.Position;

            Header = new G1tHeader
            {
                Magic = Encoding.ASCII.GetString(br.ReadBytes(4)),
                Version = Encoding.ASCII.GetString(br.ReadBytes(4)),
                FileSize = br.ReadUInt32(),
                TableOffset = br.ReadUInt32(),
                NumTextures = br.ReadUInt32(),
                Platform = (G1tPlatform)br.ReadUInt32(),
                UnkDataSize = br.ReadUInt32(),
                Unk1C = br.ReadUInt32()
            };

            if (Header.Magic != "G1TG")
                throw new Exception("Not G1TG file");

            br.BaseStream.Position = startPos + Header.TableOffset;
            uint[] offsets = new uint[Header.NumTextures];
            for (int i = 0; i < Header.NumTextures; i++)
            {
                offsets[i] = br.ReadUInt32();
            }

            Textures = new List<G1tTexture>();
            for (int i = 0; i < Header.NumTextures; i++)
            {
                br.BaseStream.Position = startPos + Header.TableOffset + offsets[i];

                var tex = new G1tTexture();
                byte mipSys = br.ReadByte();
                tex.MipCount = mipSys >> 4;
                tex.Sys = (byte)(mipSys & 0x0F);
                tex.Format = br.ReadByte();

                byte dxdy = br.ReadByte();
                tex.Width = (uint)Math.Pow(2, dxdy & 0x0F);
                tex.Height = (uint)Math.Pow(2, dxdy >> 4);

                tex.Unk3 = br.ReadBytes(4);
                tex.ExtraHeaderVersion = br.ReadByte();

                if (tex.ExtraHeaderVersion > 0)
                {
                    uint extraSize = br.ReadUInt32();
                    long extraHeaderStart = br.BaseStream.Position - 4;

                    tex.ExtraHeaderRaw = br.ReadBytes((int)extraSize);

                    if (extraSize >= 0x0C)
                    {
                        tex.ArraySize = (uint)(tex.ExtraHeaderRaw[8] >> 4);
                    }
                    if (extraSize >= 0x14)
                    {
                        tex.Width = BitConverter.ToUInt32(tex.ExtraHeaderRaw, 0x0C);
                        tex.Height = BitConverter.ToUInt32(tex.ExtraHeaderRaw, 0x10);
                    }
                    br.BaseStream.Position = extraHeaderStart + extraSize;
                }

                uint currentW = tex.Width;
                uint currentH = tex.Height;

                for (int m = 0; m < tex.MipCount; m++)
                {
                    int dataSize = CalculateDataSize(tex.Format, currentW, currentH);

                    var mip = new G1tMipMap
                    {
                        Width = currentW,
                        Height = currentH,
                        Data = br.ReadBytes(dataSize)
                    };

                    tex.MipMaps.Add(mip);

                    currentW = Math.Max(1, currentW / 2);
                    currentH = Math.Max(1, currentH / 2);
                }

                Textures.Add(tex);
            }
        }

        private int CalculateDataSize(byte format, uint w, uint h)
        {
            // BC1, BC4 - 8 bytes per block 4х4
            // BC2, BC3, BC5, BC7 - 16 bytes per block 4х4
            bool isBC1or4 = (format == 0x06 || format == 0x10 || format == 0x59 || format == 0x5C || format == 0x60 || format == 0x63);
            bool isBCOther = (format == 0x07 || format == 0x08 || format == 0x11 || format == 0x12 ||
                              format == 0x5A || format == 0x5B || format == 0x5D || format == 0x5F ||
                              format == 0x61 || format == 0x62 || format == 0x64 || format == 0x66);

            if (isBC1or4 || isBCOther)
            {
                int blockBytes = isBC1or4 ? 8 : 16;
                return (int)(Math.Max(1, (w + 3) / 4) * Math.Max(1, (h + 3) / 4) * blockBytes);
            }

            //uncompressed
            switch (format)
            {
                case 0x2A: // R8_UNORM
                case 0x18: // A8_UNORM
                    return (int)(w * h);
                case 0x19: // B5G6R5_UNORM
                case 0x1A: // B5G5R5A1_UNORM
                    return (int)(w * h * 2);
                default:
                    return (int)(w * h * 4); // RGBA8
            }
        }
    }
}
