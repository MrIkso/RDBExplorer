
namespace RDBExplorer.Core.Formats.G1M;
public class G1MIndexBufferInternal
{
    public byte[] Data;
    public int Step; // bytes per index (2 or 4)

    public G1MIndexBufferInternal(byte[] d, int s) { Data = d; Step = s; }

    public uint[] GetIndices(uint start, uint count)
    {
        var res = new uint[count];
        for (int i = 0; i < (int)count; i++)
        {
            int o = ((int)start + i) * Step;
            if (o + Step > Data.Length) 
                break;
            res[i] = (Step == 4)
                ? BitConverter.ToUInt32(Data, o)
                : BitConverter.ToUInt16(Data, o);
        }
        return res;
    }
}
