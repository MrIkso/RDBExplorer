namespace RDBExplorer.Core.Formats.G1M;
public class G1MSemanticInternal
{
    public ushort BufIdx;   // index into layout's BufferIndices list
    public ushort Offset;   // byte offset within the buffer stride
    public G1MDataFormat Format;   // data format enum  (see G1MDataFormat)
    public byte RawType;  // semantic enum     (see G1MSemanticType)
    public byte Layer;    // semantic index / UV layer etc.

    public G1MSemanticType Type => (G1MSemanticType)RawType;
    public int Index => Layer;
}
