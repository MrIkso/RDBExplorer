namespace RDBExplorer.Core.G1T
{
    public static class G1tFormats
    {
        private static readonly Dictionary<byte, string> DxgiNames = new Dictionary<byte, string>
        {
            { 0x06, "BC1_UNORM" },
            { 0x08, "BC3_UNORM" },
            { 0x59, "BC1_UNORM" },
            { 0x5B, "BC3_UNORM" },
            { 0x5D, "BC5_UNORM" }, // Normal Maps
            { 0x5F, "BC7_UNORM" },
            { 0x00, "R8G8B8A8_UNORM" }
        };

        public static string GetDxgiName(byte fmt)
        {
            return DxgiNames.ContainsKey(fmt) ? DxgiNames[fmt] : $"Unknown_0x{fmt:X2}";
        }
    }
}
