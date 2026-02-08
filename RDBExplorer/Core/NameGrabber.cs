using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDBExplorer.Core
{
    internal class NameGrabber
    {
        public Dictionary<uint, string> GrabbedNames = new Dictionary<uint, string>();

        public void Load(byte[] data, uint hashName, bool isGrabMagic)
        {
            if (data == null || data.Length < 16)
                return;

            using (var ms = new MemoryStream(data))
            {
                Load(ms, hashName, isGrabMagic);
            }
        }

        public void Load(Stream stream, uint hashName, bool isGrabMagic)
        {
            using (var br = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true))
            {
                Read(br, hashName, isGrabMagic);
            }
        }

        private void Read(BinaryReader br, uint hashName, bool isGrabMagic)
        {
            long startPos = br.BaseStream.Position;
            uint magicNum = br.ReadUInt32();

            if (isGrabMagic)
            {
                string magicName = new string(new char[] {
                    (char)(magicNum & 0xFF),         // 'I' (0x49)
                    (char)((magicNum >> 8) & 0xFF),  // 'D' (0x44)
                    (char)((magicNum >> 16) & 0xFF), // 'R' (0x52)
                    (char)((magicNum >> 24) & 0xFF)  // 'K' (0x4B)
                });
                GrabbedNames[hashName] = magicName;
            }
            else
            {
                uint version = br.ReadUInt32();
                uint size = br.ReadUInt32();
                int stringLength = br.ReadInt32();

                if (stringLength > 0 && stringLength < 1024 && br.BaseStream.Position + stringLength <= br.BaseStream.Length)
                {
                    string internalPath = Encoding.ASCII.GetString(br.ReadBytes(stringLength)).TrimEnd('\0');

                    if (!string.IsNullOrWhiteSpace(internalPath))
                    {
                        GrabbedNames[hashName] = internalPath;
                    }
                }
            }
        }


        public void SaveToFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Hash,InternalName");

            foreach (var kvp in GrabbedNames)
            {
                sb.AppendLine($"0x{kvp.Key:X8},\"{kvp.Value}\"");
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
