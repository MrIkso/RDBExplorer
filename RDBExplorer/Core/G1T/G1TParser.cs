using System.IO.Compression;
using System.Text;

namespace RDBExplorer.Core.G1T
{
    public class G1TParser
    {
        public G1TFile G1TFile { get; set; }

        public void Load(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            Load(data);
        }

        public void Load(byte[] data)
        {
            G1TFile = new G1TFile();
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms, Encoding.ASCII))
            {
                long startPos = br.BaseStream.Position;

                G1TFile.Header = new G1THeader
                {
                    Magic = br.ReadUInt32(),
                    Version = br.ReadUInt32(),
                    FileSize = br.ReadUInt32(),
                    TableOffset = br.ReadUInt32(),
                    NumTextures = br.ReadUInt32(),
                    Platform = (G1TPlatform)br.ReadUInt32(),
                    MetadataSize = br.ReadUInt32(),
                    Unk1C = br.ReadUInt32()
                };

                if (G1TFile.Header.Magic != 0x47315447)
                    throw new Exception("Invalid G1T magic");


                uint[] normalFlags = new uint[G1TFile.Header.NumTextures];
                for (int i = 0; i < G1TFile.Header.NumTextures; i++)
                {
                    normalFlags[i] = br.ReadUInt32();
                }

                if (G1TFile.Header.MetadataSize > 0)
                {
                    G1TFile.Header.GlobalMetadata = br.ReadBytes((int)G1TFile.Header.MetadataSize);
                }

                br.BaseStream.Position = startPos + G1TFile.Header.TableOffset;
                uint[] offsets = new uint[G1TFile.Header.NumTextures];
                for (int i = 0; i < G1TFile.Header.NumTextures; i++)
                {
                    offsets[i] = br.ReadUInt32();
                }

                G1TFile.Textures.Clear();
                for (int i = 0; i < G1TFile.Header.NumTextures; i++)
                {
                    br.BaseStream.Position = startPos + G1TFile.Header.TableOffset + offsets[i];
                    var tex = new G1TTexture { NormalMapFlags = normalFlags[i] };

                    byte mipSys = br.ReadByte();
                    int mipCount = mipSys >> 4;
                    tex.LoadType = (G1TLoadType)(byte)(mipSys & 0x0F);
                    tex.Format = (G1TFormat)br.ReadByte();

                    byte dxdy = br.ReadByte();

                    tex.Width = (uint)Math.Pow(2, dxdy & 0x0F);
                    tex.Height = (uint)Math.Pow(2, dxdy >> 4);

                    byte d_ex = br.ReadByte();
                    tex.Depth = (uint)(1 << (d_ex & 0x0F));
                    tex.Metadata = br.ReadBytes(3);
                    tex.ExtraHeaderVersion = br.ReadByte();

                    if (tex.ExtraHeaderVersion > 0)
                    {
                        tex.ExtraHeaderSize = br.ReadUInt32();
                        long extraStart = br.BaseStream.Position - 4;
                        br.BaseStream.Position = extraStart;
                        tex.ExtraHeaderRaw = br.ReadBytes((int)tex.ExtraHeaderSize);

                        tex.ZScale = BitConverter.ToUInt32(tex.ExtraHeaderRaw, 4);
                        ushort exInfo = BitConverter.ToUInt16(tex.ExtraHeaderRaw, 8);
                        tex.ExFaces = (uint)(exInfo & 0x000F);
                        uint exArray = (uint)(exInfo >> 4);

                        if (tex.ExtraHeaderRaw.Length > 10)
                        {
                            tex.EX_SwizzleType = (EX_SWIZZLE_TYPE)tex.ExtraHeaderRaw[10];
                        }

                        if (tex.LoadType == G1TLoadType.PLANE_ARRAY || tex.LoadType == G1TLoadType.CUBE_ARRAY)
                        {
                            tex.ArraySize = exArray;
                        }
                        else if (tex.LoadType == G1TLoadType.VOLUME)
                        {
                            tex.ArraySize = (uint)(1 << (int)exArray);
                        }

                        if (tex.ExtraHeaderSize >= 0x10)
                        {
                            tex.Width = BitConverter.ToUInt32(tex.ExtraHeaderRaw, 0x0C);
                        }
                        if (tex.ExtraHeaderSize >= 0x14)
                        {
                            tex.Height = BitConverter.ToUInt32(tex.ExtraHeaderRaw, 0x10);
                        }

                        br.BaseStream.Position = extraStart + tex.ExtraHeaderSize;
                    }

                    uint totalLayers = tex.GetTotalLayers();
                    int faces = 1;
                    if (tex.ExtraHeaderVersion > 0 && tex.ExFaces != 0)
                    {
                        faces = (int)tex.ExFaces;
                    }

                    byte[] textureRawData;
                    if (tex.EX_SwizzleType == EX_SWIZZLE_TYPE.ZLIB_COMPRESSED)
                    {
                        textureRawData = DecompressZlibTexture(br, (int)tex.Depth, (int)totalLayers, faces);
                    }
                    else
                    {
                        long dataSize = 0;
                        uint currW = tex.Width, currH = tex.Height;
                        for (int m = 0; m < Math.Max(1, mipCount); m++)
                        {
                            dataSize += G1TFormatUtils.CalculateMipSize(tex, currW, currH) * tex.Depth * totalLayers;
                            currW = Math.Max(1, currW / 2); currH = Math.Max(1, currH / 2);
                        }
                        textureRawData = br.ReadBytes((int)dataSize);
                    }

                    using (var texMs = new MemoryStream(textureRawData))
                    using (var texBr = new BinaryReader(texMs))
                    {
                        uint currW = tex.Width;
                        uint currH = tex.Height;
                        uint currD = tex.Depth;
                        int mipsToRead = mipCount == 0 ? 1 : mipCount;

                        for (int m = 0; m < mipsToRead; m++)
                        {
                            var mip = new G1TMipMap { Width = currW, Height = currH };
                            int singleLayerSize = G1TFormatUtils.CalculateMipSize(tex, currW, currH);

                            for (int l = 0; l < totalLayers; l++)
                            {
                                mip.Layers.Add(texBr.ReadBytes(singleLayerSize * (int)currD));
                            }

                            tex.MipMaps.Add(mip);
                            currW = Math.Max(1, currW / 2);
                            currH = Math.Max(1, currH / 2);
                            currD = Math.Max(1, currD / 2);
                        }
                    }
                    G1TFile.Textures.Add(tex);
                }
            }
        }

        private byte[] DecompressZlibTexture(BinaryReader br, int depth, int planeCount, int facesCount)
        {
            br.ReadBytes(4); // magic
            int tableSize = br.ReadInt32();
            br.ReadInt32(); // Unk
            int windowSize = br.ReadInt32();
            int meta1Count = br.ReadInt32();
            int chunkCount = br.ReadInt32();
            int meta2Count = br.ReadInt32();
            int hasUncompChunk = br.ReadInt32();
            int uncompChunkSize = br.ReadInt32();

            for (int i = 0; i < meta1Count * depth * planeCount * facesCount; i++)
            {
                int COMPED_META_DATA1 = br.ReadInt32();
                int COMPED_META_DATA2 = br.ReadInt32();
                int COMPED_META_DATA3 = br.ReadInt32();
                int COMPED_META_DATA4 = br.ReadInt32();
            }
            for (int i = 0; i < meta2Count * depth * planeCount * facesCount; i++)
            {
                int COMPED_META2_DATA1 = br.ReadInt32();
                int COMPED_META2_DATA2 = br.ReadInt32();
                int COMPED_META2_DATA3 = br.ReadInt32();
                int COMPED_META2_DATA4 = br.ReadInt32();
            }
            var chunks = new List<(int offset, int size)>();
            for (int i = 0; i < chunkCount; i++)
            {
                chunks.Add((br.ReadInt32(), br.ReadInt32()));
            }

            if (hasUncompChunk > 0)
            {
                chunks.Add((br.ReadInt32(), br.ReadInt32()));
            }

            using (var outMs = new MemoryStream())
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    br.BaseStream.Position = chunks[i].offset;
                    long chunkEnd = chunks[i].offset + chunks[i].size;

                    if (i == chunkCount && hasUncompChunk > 0)
                    {
                        outMs.Write(br.ReadBytes(chunks[i].size), 0, chunks[i].size);
                    }
                    else
                    {
                        while (br.BaseStream.Position < chunkEnd)
                        {
                            if (br.BaseStream.Position + 4 > br.BaseStream.Length)
                                break;

                            uint compressedBlockSize = br.ReadUInt32();
                            if (compressedBlockSize == 0)
                            {
                                break;
                            }
                            byte[] compressedData = br.ReadBytes((int)compressedBlockSize);

                            using (var compMs = new MemoryStream(compressedData))
                            using (var zs = new ZLibStream(compMs, CompressionMode.Decompress))
                            {
                                try
                                {
                                    zs.CopyTo(outMs);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Zlib error at chunk {i}: {ex.Message}");
                                    break;
                                }
                            }
                        }
                    }
                }
                return outMs.ToArray();
            }
        }

        public byte[] Save()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms, Encoding.ASCII))
            {
                bw.Write(G1TFile.Header.Magic);
                bw.Write(G1TFile.Header.Version);
                long fileSizePos = ms.Position;
                bw.Write((uint)0);
                long tableOffsetPos = ms.Position;
                bw.Write((uint)0);
                bw.Write((uint)G1TFile.Textures.Count);
                bw.Write((uint)G1TFile.Header.Platform);
                bw.Write((uint)(G1TFile.Header.GlobalMetadata?.Length ?? 0));
                bw.Write(G1TFile.Header.Unk1C);

                foreach (var tex in G1TFile.Textures)
                {
                    bw.Write(tex.NormalMapFlags);
                }

                if (G1TFile.Header.GlobalMetadata != null)
                {
                    bw.Write(G1TFile.Header.GlobalMetadata);
                }

                uint tableStartRelative = (uint)ms.Position;
                bw.Write(new byte[G1TFile.Textures.Count * 4]);

                uint[] offsets = new uint[G1TFile.Textures.Count];

                for (int i = 0; i < G1TFile.Textures.Count; i++)
                {
                    offsets[i] = (uint)ms.Position - tableStartRelative;
                    var tex = G1TFile.Textures[i];

                    byte mipSys = (byte)(tex.MipMaps.Count << 4 | (byte)tex.LoadType & 0x0F);
                    bw.Write(mipSys);
                    bw.Write((byte)tex.Format);
                    bw.Write(tex.GetDxDyByte());

                    byte depthByte = (byte)((int)Math.Log(tex.Depth, 2) & 0x0F);
                    bw.Write(depthByte);

                    bw.Write(tex.Metadata);
                    bw.Write(tex.ExtraHeaderVersion);

                    if (tex.ExtraHeaderVersion > 0)
                    {
                        UpdateExtraHeaderRaw(tex);
                        bw.Write(tex.ExtraHeaderRaw);
                    }

                    foreach (var mip in tex.MipMaps)
                    {
                        foreach (var layerData in mip.Layers)
                        {
                            bw.Write(layerData);
                        }
                    }
                }

                uint totalSize = (uint)ms.Length;
                ms.Position = fileSizePos;
                bw.Write(totalSize);
                bw.Write(tableStartRelative);

                ms.Position = tableStartRelative;
                foreach (uint offset in offsets)
                {
                    bw.Write(offset);
                }

                return ms.ToArray();
            }
        }

        private void UpdateExtraHeaderRaw(G1TTexture tex)
        {
            if (tex.ExtraHeaderRaw == null || tex.ExtraHeaderRaw.Length < 12)
                return;

            if (tex.ExtraHeaderRaw.Length >= 0x10)
            {
                byte[] wBytes = BitConverter.GetBytes(tex.Width);
                Array.Copy(wBytes, 0, tex.ExtraHeaderRaw, 12, 4);
            }
            if (tex.ExtraHeaderRaw.Length >= 0x14)
            {
                byte[] hBytes = BitConverter.GetBytes(tex.Height);
                Array.Copy(hBytes, 0, tex.ExtraHeaderRaw, 16, 4);
            }

            byte arrayByte = (byte)(tex.ExtraHeaderRaw[8] & 0x0F | (byte)(tex.ArraySize << 4));
            tex.ExtraHeaderRaw[8] = arrayByte;
            if (tex.ExtraHeaderRaw.Length > 10)
            {
                tex.ExtraHeaderRaw[10] = 0x00;
                tex.EX_SwizzleType = 0;
            }
        }

        public void SaveToFile(string path)
        {
            byte[] data = Save();
            File.WriteAllBytes(path, data);
        }

        public void UpdateTexture(int index, G1TTexture newTexture)
        {
            if (index < 0 || index >= G1TFile.Textures.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            G1TFile.Textures[index] = newTexture;
        }
    }
}