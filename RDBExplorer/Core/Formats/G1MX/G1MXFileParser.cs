using RDBExplorer.Utils;
using System.Text;

namespace RDBExplorer.Core.Formats.G1MX
{
    public class G1MXFileParser
    {
        public G1MXFile Parse(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            return Parse(data);
        }

        public G1MXFile Parse(byte[] data) { 
            using (MemoryStream fs = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                G1MXFile file = new G1MXFile();

                file.KG1M = ReadKG1M(reader);

                long currentPos = reader.BaseStream.Position;
                long alignedPos = currentPos + 15 & ~15;
                reader.BaseStream.Position = alignedPos;

                file.G1MX = ReadG1MX(reader);

                return file;
            }
        }

        private KG1M ReadKG1M(BinaryReader reader)
        {
            KG1M kgm = new KG1M();
            kgm.Magic = reader.ReadEncodedString(4);
            kgm.Version = reader.ReadUInt32();
            kgm.HeaderSize = reader.ReadUInt32();
            kgm.TextLen = reader.ReadUInt32();
            kgm.Text = reader.ReadEncodedString((int)kgm.TextLen);
            return kgm;
        }

        private G1MX ReadG1MX(BinaryReader reader)
        {
            G1MX g1mx = new G1MX();
            g1mx.Magic = reader.ReadEncodedString(4);
            g1mx.Version = reader.ReadEncodedString(4);
            g1mx.BlockSize = reader.ReadUInt32();
            g1mx.DataStartPointer = reader.ReadUInt32();

            g1mx.G1MXF = ReadG1MXF(reader);
            return g1mx;
        }

        private G1MXF ReadG1MXF(BinaryReader reader)
        {
            G1MXF f = new G1MXF();
            f.Magic = reader.ReadEncodedString(4);
            f.Version = reader.ReadEncodedString(4);
            f.HeaderSize = reader.ReadUInt32();
            f.Unk1 = reader.ReadUInt32();
            f.Unk2 = reader.ReadUInt32();
            f.Zero = reader.ReadUInt32();

            f.GMXM = ReadGMXM(reader);
            return f;
        }

        private GMXM ReadGMXM(BinaryReader reader)
        {
            GMXM m = new GMXM();
            m.Magic = reader.ReadEncodedString(4);
            m.Version = reader.ReadEncodedString(4);
            m.DataSize = reader.ReadUInt32();
            m.ElementCount = reader.ReadUInt32();
            m.HeaderSize = reader.ReadUInt32();

            // Dependency List (elementCount - 1)
            m.DependencyList = new uint[m.ElementCount - 1];
            for (int i = 0; i < m.ElementCount - 1; i++)
            {
                m.DependencyList[i] = reader.ReadUInt32();
            }

            for (int i = 0; i < m.ElementCount; i++)
            {
                m.G1M_ModelsList.Add(ReadG1M(reader));
            }
            return m;
        }

        private G1M ReadG1M(BinaryReader reader)
        {
            G1M g = new G1M();
            long modelStartPos = reader.BaseStream.Position;
            g.Magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
            g.Version = Encoding.ASCII.GetString(reader.ReadBytes(4));
            g.DataSize = reader.ReadUInt32();
            g.HeaderSize = reader.ReadUInt32();
            g.Unk = reader.ReadUInt32();
            g.ChunkCount = reader.ReadUInt32();

            for (int i = 0; i < g.ChunkCount; i++)
            {
                g.Chunks.Add(ReadChunk(reader));
            }
            long oldestStartPos = reader.BaseStream.Position;
            reader.BaseStream.Position = modelStartPos;
            
            g.Data = reader.ReadBytes((int)g.DataSize);
            
            reader.BaseStream.Position = oldestStartPos;

            return g;
        }

        private GResourceChunk ReadChunk(BinaryReader reader)
        {
            GResourceChunk c = new GResourceChunk();
            c.Magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
            c.Version = Encoding.ASCII.GetString(reader.ReadBytes(4));
            c.DataSize = reader.ReadUInt32();

            int payloadSize = (int)c.DataSize - 12;
            if (payloadSize > 0)
            {
                c.Data = reader.ReadBytes(payloadSize);
            }
            else
            {
                c.Data = new byte[0];
            }
            return c;
        }
    }

}
