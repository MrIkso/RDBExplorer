using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDBExplorer.Core.Models
{
    // 12 bytes size
    public struct KRDIParam
    {
        public int Type;
        public uint Unk;
        public int HashName;
    }

    // 56 bytes size
    public struct KRDIHeader
    {
        public string Magic;
        public string Version;
        public long AllBlockSize;
        public long CompressedSize;
        public long UncompressedSize;
        public int ParamDataSize;
        public int HashName;
        public int HashType;
        public RDBFlags Flags;
        public uint ResourceId;
        public int ParamCount;

    }
}
