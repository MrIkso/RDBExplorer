namespace RDBExplorer.Core.Formats.G1M;
public class G1MTextureRef
{
    public ushort Index;       // index in G1T
    public ushort Layer;       // TEXCOORD layer
    public ushort TextureType; // 0=diffuse, 1=normal, 2=specular...
}
