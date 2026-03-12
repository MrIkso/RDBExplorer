namespace RDBExplorer.Core.Formats.G1M;
public class G1MSemanticInternal
{
    public ushort BufIdx;   // index into layout's BufferIndices list
    public ushort Offset;   // byte offset within the buffer stride
    public EG1MGVADatatype Format;   // data format enum  (see G1MDataFormat)
    public G1MSemanticType Type;
    public byte Layer;    // semantic index / UV layer etc.

    public int Index => Layer;
}
