using RDBExplorer.Core.Formats.ObjectDatabaseFile;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Utils.JsonConverters
{
    public class KidsOdbColumnConverter : JsonConverter<KidsOdbColumn>
    {
        public override KidsOdbColumn Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Read is not implemented. Use default if needed.");
        }

        public override void Write(Utf8JsonWriter writer, KidsOdbColumn value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("type", value.Type.ToString());

            writer.WriteNumber("row_count", value.RowCount);
            writer.WriteString("property_ktid", $"0x{value.PropertyKTID:X8}");
            if (value.PropertyName != null)
            {
                writer.WriteString("property_name", value.PropertyName);
            }

            writer.WritePropertyName("values");
            writer.WriteStartArray();

            bool isHashProperty = value.PropertyName != null &&
                                  value.PropertyName.Contains("Hash", StringComparison.OrdinalIgnoreCase);

            foreach (var item in value.Values)
            {
                if (item == null)
                {
                    writer.WriteNullValue();
                    continue;
                }

                if (isHashProperty && (item is uint || item is ulong))
                {
                    if (item is uint uVal)
                        writer.WriteStringValue($"0x{uVal:X8}");
                    else if (item is ulong ulVal)
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
            writer.WriteEndObject();
        }

    }
}
