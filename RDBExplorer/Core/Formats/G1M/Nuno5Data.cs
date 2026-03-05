using OpenTK.Mathematics;
using System.Runtime.InteropServices;
namespace RDBExplorer.Core.Formats.G1M;
public interface INunoEntry
{
    uint ParentID { get; }
    List<Vector4> ControlPoints { get; }
    List<NunInfluence> Influences { get; }
}

public struct NunInfluence
{
    public int P1, P2, P3, P4;
    public float P5, P6;
    public NunInfluence(Nuno5Influence n5)
    {
        P1 = n5.P1;
        P2 = n5.P2;
        P3 = n5.P3;
        P4 = n5.P4;
        P5 = n5.P5;
        P6 = 0;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Nuno5Influence
{
    public int P1, P2, P3, P4;
    public float P5; // 20 bytes
}

public class Nuno1Data : INunoEntry
{
    public uint ParentID { get; set; }
    public List<Vector4> ControlPoints { get; set; } = new List<Vector4>();
    public List<NunInfluence> Influences { get; set; } = new List<NunInfluence>();
}

public class Nuno3Data : INunoEntry
{
    public uint ParentID { get; set; }
    public List<Vector4> ControlPoints { get; set; } = new List<Vector4>();
    public List<NunInfluence> Influences { get; set; } = new List<NunInfluence>();
}

public class Nuno5Data : INunoEntry
{
    public uint ParentID { get; set; }
    public List<Vector4> ControlPoints { get; set; } = new List<Vector4>();
    public List<NunInfluence> Influences { get; set; } = new List<NunInfluence>();
    public uint EntryID { get; set; }
    public int ParentSetID { get; set; } = -1;
}
