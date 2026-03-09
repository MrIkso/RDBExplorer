namespace RDBExplorer.Core.Formats.G1M;
public class G1MMeshInternal
{
    public ushort ClothID;
    public uint ExternalID;
    public List<uint> SubmeshIndices = new(); // indexes в Submeshes[]
}
