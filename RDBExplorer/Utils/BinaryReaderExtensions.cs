using System.Runtime.InteropServices;
using System.Text;

namespace RDBExplorer.Utils
{
    public static class BinaryReaderExtensions
    {
        public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }
            return encoding.GetString(bytes.ToArray());
        }


        /// <summary>
        /// Reads a null-terminated UTF-16 (Unicode) string.
        /// Stops at the first 0x0000 occurrence.
        /// </summary>
        public static string ReadNullTerminatedUnicode(this BinaryReader reader)
        {
            List<byte> byteList = new List<byte>();

            try
            {
                while (true)
                {
                    // Read 2 bytes (one UTF-16 character)
                    byte b1 = reader.ReadByte();
                    byte b2 = reader.ReadByte();

                    // Check for null terminator (00 00)
                    if (b1 == 0x00 && b2 == 0x00)
                    {
                        break;
                    }

                    byteList.Add(b1);
                    byteList.Add(b2);
                }
            }
            catch (EndOfStreamException)
            {
                // Handle cases where the file might end without a null terminator
            }

            return Encoding.Unicode.GetString(byteList.ToArray());
        }

        public static string ReadEncodedString(this BinaryReader reader, int length, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            var bytes = reader.ReadBytes(length);
            return encoding.GetString(bytes).TrimEnd('\0');
        }

        public static (string text, bool isUtf16) ReadAutoEncodedString(this BinaryReader reader)
        {
            long startPos = reader.BaseStream.Position;
            byte[] peek = reader.ReadBytes(2);
            reader.BaseStream.Position = startPos;

            if (peek.Length < 2) return ("", false);

            bool isUtf16 = false;
            if (peek[1] == 0x00)
            {
                isUtf16 = true;
            }
 
            else if (peek[1] < 0x20 || peek[1] > 0x7E)
            {
                isUtf16 = true;
            }
            else
            {
                isUtf16 = false;
            }

            List<byte> bytes = new List<byte>();
            if (isUtf16)
            {
                while (reader.BaseStream.Position + 1 < reader.BaseStream.Length)
                {
                    byte b1 = reader.ReadByte();
                    byte b2 = reader.ReadByte();
                    if (b1 == 0 && b2 == 0) break;
                    bytes.Add(b1);
                    bytes.Add(b2);
                }
                return (Encoding.Unicode.GetString(bytes.ToArray()), true);
            }
            else
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    byte b = reader.ReadByte();
                    if (b == 0) break;
                    bytes.Add(b);
                }
                return (Encoding.UTF8.GetString(bytes.ToArray()), false);
            }
        }

        public static T ReadStruct<T>(this BinaryReader br) where T : struct
        {
            byte[] bytes = br.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static void Align(this BinaryReader br, int size)
        {
            long currentPosition = br.BaseStream.Position;
            long alignedPosition = AlignmentHelper.Align(currentPosition, size);
            br.BaseStream.Position = alignedPosition;
        }
    }
}
