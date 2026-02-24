using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Utils.JsonConverters
{
    public class JsonKidsOdbValuesConverter : JsonConverter<List<object>>
    {
        public override List<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray token");

            var list = new List<object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return list;

                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        string s = reader.GetString();
                        if (s != null && s.StartsWith("0x"))
                        {
                            string hexPart = s.Substring(2);
                            if (hexPart.Length <= 8)
                            {
                                if (uint.TryParse(hexPart, NumberStyles.HexNumber, null, out uint uVal))
                                    list.Add(uVal);
                                else
                                    list.Add(s);
                            }
                            else
                            {
                                if (ulong.TryParse(hexPart, NumberStyles.HexNumber, null, out ulong ulVal))
                                    list.Add(ulVal);
                                else
                                    list.Add(s);
                            }
                        }
                        else
                        {
                            list.Add(s);
                        }
                        break;

                    case JsonTokenType.Number:
                        if (reader.TryGetInt32(out int iVal))
                            list.Add(iVal);
                        else if (reader.TryGetInt64(out long lVal))
                            list.Add(lVal);
                        else
                            list.Add(reader.GetDouble());
                        break;

                    case JsonTokenType.True:
                        list.Add(true);
                        break;

                    case JsonTokenType.False:
                        list.Add(false);
                        break;

                    case JsonTokenType.Null:
                        list.Add(null);
                        break;

                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                        {
                            list.Add(doc.RootElement.Clone());
                        }
                        break;


                    default:
                        list.Add(JsonSerializer.Deserialize<object>(ref reader, options));
                        break;
                }
            }

            throw new JsonException("Unexpected end of JSON");
        }

        public override void Write(Utf8JsonWriter writer, List<object> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                if (item is uint uVal)
                {
                    writer.WriteStringValue($"0x{uVal:X8}");
                }
                else if (item is ulong ulVal)
                {
                    writer.WriteStringValue($"0x{ulVal:X16}");
                }
                else if (item is byte bVal)
                {
                    writer.WriteNumberValue(bVal);
                }
                else
                {
                    JsonSerializer.Serialize(writer, item, item.GetType(), options);
                }
            }
            writer.WriteEndArray();
        }
    }
}
