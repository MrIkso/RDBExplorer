using RDBExplorer.Core.Models;
using RDBExplorer.Core.Wrappers;

namespace RDBExplorer.Core
{
    public static class ResourceFactory
    {
        public static IResourceParser? CreateParser(KTFileType type)
        {
            return type switch
            {
                KTFileType.G1MXFile => new G1MXWrapper(),
                KTFileType.KTIDFileBinary => new KTIDWrapper(),
                KTFileType.ObjectDatabaseFile => new KidsObjDbParserWrapper(),
                KTFileType.G1COFile => new G1COWrapper(),
                KTFileType.OIDBindTableBinaryFile => new OIDWrapper(),
                KTFileType.OIDSQTBindTableBinaryFile => new OIDSQWrapper(),
                KTFileType.OBOROStaticResourceBinaryFile => new OBOROWrapper(),
                KTFileType.PartsModelGroupBindTableBinaryFile => new GRPWrapper(),
                KTFileType.StaticScreenLayoutTexInfoFile => new TextInfoWrapper(),
                _ => null /*throw new NotSupportedException($"This file type: {type} not supported!")*/
            };
        }

        public static IResourceParser? GetLoadedParser(KTFileType type, byte[] data)
        {
            var parser = CreateParser(type);
            if (parser != null)
            {
                parser.Load(data);
                return parser;
            }
            return null;
        }
    }

}
