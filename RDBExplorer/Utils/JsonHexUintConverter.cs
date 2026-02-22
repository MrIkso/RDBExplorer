using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Utils
{
    public class JsonHexUintConverter : JsonConverter<uint>
    {
        public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string s = reader.GetString();
            if (s.StartsWith("0x"))
                return uint.Parse(s.Substring(2), NumberStyles.HexNumber);
            return uint.Parse(s);
        }

        public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"0x{value:X8}");
        }
    }
}
