using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace RDBExplorer.Utils
{
    public class Signature
    {
        public byte[] Pattern { get; }
        public bool[] Mask { get; }
        public string Extension { get; }

        public Signature(string hexMask, string extension)
        {
            Extension = extension;
            string[] parts = hexMask.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Pattern = new byte[parts.Length];
            Mask = new bool[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "??" || parts[i] == "?")
                {
                    Pattern[i] = 0x00;
                    Mask[i] = false;
                }
                else
                {
                    Pattern[i] = byte.Parse(parts[i], NumberStyles.HexNumber);
                    Mask[i] = true;
                }
            }
        }

        public bool IsMatch(ReadOnlySpan<byte> data)
        {
            if (data.Length < Pattern.Length) return false;

            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Mask[i] && data[i] != Pattern[i])
                    return false;
            }
            return true;
        }
    }

    public static class FileTypeDetector
    {
        private static readonly List<Signature> Signatures = new List<Signature>
        {
            new Signature("? 50 4E 47", ".png"),
            AddAsciiSignature("GT1G", ".g1t"),
            AddAsciiSignature("G2A_PACK", ".g2a"),
            AddAsciiSignature("_S2G", ".g2s"),
            AddAsciiSignature("_N1G", ".g1n"),
            AddAsciiSignature("TMG_PACK", ".tmg"),
            AddAsciiSignature("TRMD", ".dmrt"),
            AddAsciiSignature("DMPP", ".dmpp"),
          
        };

        public static string DetectExtension(byte[] data)
        {
            if (data == null || data.Length < 4)
                return ".dat";

            ReadOnlySpan<byte> header = data.Length > 16 ? data.AsSpan(0, 16) : data.AsSpan();

            foreach (var sig in Signatures)
            {
                if (sig.IsMatch(header))
                {
                    return sig.Extension;
                }
            }

            return ".dat";
        }

        public static Signature AddAsciiSignature(string ascii, string ext)
        {
            string hex = string.Join(" ", Encoding.ASCII.GetBytes(ascii).Select(b => b.ToString("X2")));
            return new Signature(hex, ext);
        }
    }
}
