using System.IO.Compression;
using System.Text;

namespace RDBExplorer.Utils
{
    public static class CompressUtils
    {
        public static byte[] DecompressZlibChunk(byte[] compressedData, int decompressedSize)
        {
            using (var msInput = new MemoryStream(compressedData))
            using (var msOutput = new MemoryStream(decompressedSize))
            {
                using (var zlib = new ZLibStream(msInput, CompressionMode.Decompress))
                {
                    zlib.CopyTo(msOutput);
                }
                return msOutput.ToArray();
            }
        }

        public static byte[] DecompressAssetsBin(byte[] input)
        {
            if (input == null || input.Length < 6)
            {
                return Array.Empty<byte>();
            }

            using (var msInput = new MemoryStream(input))
            using (var msOutput = new MemoryStream())
            using (var reader = new BinaryReader(msInput, Encoding.Default, true))
            {
                List<long> chunksOffsets = new List<long>();

                // search all compressed blocks
                while (msInput.Position <= msInput.Length - 6)
                {
                    // 16 byte aligment
                    if (msInput.Position % 16 != 0)
                        msInput.Position = (msInput.Position + 15) & ~15;

                    if (msInput.Position > msInput.Length - 6)
                        break;

                    long currentPos = msInput.Position;
                    uint uncompressedSize = reader.ReadUInt32();
                    ushort magic = reader.ReadUInt16();

                    // 78 9C
                    if (uncompressedSize > 0 && uncompressedSize <= 0x10000 && magic == 0x9C78)
                    {
                        Console.WriteLine($"Block size {uncompressedSize}");
                        chunksOffsets.Add(currentPos);
                        msInput.Position = currentPos + 16;
                    }
                    else
                    {
                        msInput.Position = currentPos + 16;
                    }
                }

               /* Console.WriteLine($"found blocks: {chunksOffsets.Count}");*/

                // decompress blocks
                for (int i = 0; i < chunksOffsets.Count; i++)
                {
                    long startOffset = chunksOffsets[i];
                    long nextOffset = (i + 1 < chunksOffsets.Count) ? chunksOffsets[i + 1] : msInput.Length;

                    msInput.Position = startOffset;
                    uint expectedUncompressedSize = reader.ReadUInt32();

                    int compressedSize = (int)(nextOffset - startOffset - 4);
                    if (compressedSize <= 0)
                    {
                        continue;
                    }

                    byte[] compressedData = reader.ReadBytes(compressedSize);

                    using (var msChunk = new MemoryStream(compressedData))
                    using (var zlib = new ZLibStream(msChunk, CompressionMode.Decompress))
                    {
                        byte[] decompressedBuffer = new byte[expectedUncompressedSize];
                        int bytesRead = 0;

                        while (bytesRead < expectedUncompressedSize)
                        {
                            int r = zlib.Read(decompressedBuffer, bytesRead, (int)expectedUncompressedSize - bytesRead);
                            if (r <= 0)
                            {
                                break;
                            }
                            bytesRead += r;
                        }
                        msOutput.Write(decompressedBuffer, 0, bytesRead);
                    }
                }

                return msOutput.ToArray();
            }
        }

        /// <summary>
        /// NOT USABLE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] CompressAssetsBin(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                return Array.Empty<byte>();
            }

            // Use 42 KB for the chunk size as requested (Must be <= 64 KB for your decompressor)
            const int CHUNK_SIZE = 32 * 1024;
            // Internal paging size for the zlib write process
            const int INTERNAL_PAGE_SIZE = 16 * 1024;

            using (var msOutput = new MemoryStream())
            {
                int position = 0;

                while (position < input.Length)
                {
                    // Ensure 16-byte alignment for the START of the block
                    
                    long currentPos = msOutput.Position;
                    int paddingToAlign = (int)((16 - (currentPos % 16)) % 16);
                    if (paddingToAlign > 0)
                    {
                        msOutput.Write(new byte[paddingToAlign], 0, paddingToAlign);
                    }

                    // Determine the size of the current raw data chunk
                    int currentUncompressedSize = Math.Min(CHUNK_SIZE, input.Length - position);

                    // Write the Block Header: Uncompressed Size (4 bytes)
                    msOutput.Write(BitConverter.GetBytes((uint)currentUncompressedSize), 0, 4);

                    // Compress the data using ZLib
                    // The Decompressor expects the magic 0x9C78 (78 9C) immediately after the 4-byte size
                    using (var msTemp = new MemoryStream())
                    {
                        using (var zlib = new ZLibStream(msTemp, CompressionLevel.Optimal, leaveOpen: true))
                        {
                            int bytesWrittenInChunk = 0;
                            while (bytesWrittenInChunk < currentUncompressedSize)
                            {
                                int toWrite = Math.Min(INTERNAL_PAGE_SIZE, currentUncompressedSize - bytesWrittenInChunk);
                                zlib.Write(input, position + bytesWrittenInChunk, toWrite);
                                bytesWrittenInChunk += toWrite;
                            }
                        }

                        // Write the compressed payload to the main stream
                        byte[] compressedData = msTemp.ToArray();
                        msOutput.Write(compressedData, 0, compressedData.Length);
                    }

                    // Move to the next chunk of raw data
                    position += currentUncompressedSize;
                }

                return msOutput.ToArray();
            }
        }
    }
}
