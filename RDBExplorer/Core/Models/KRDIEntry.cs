namespace RDBExplorer.Core.Models
{
    public class KRDIEntry
    {
        public KRDIHeader Header { get; set; }
        public List<KRDIParam> KRDIParams { get; set; }
        public byte[] ParamData { get; set; }
    }
}
