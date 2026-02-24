using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Utils.JsonConverters
{
    public class JsonVectorConverters : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) 
                throw new JsonException("Expected array for Vector2");

            reader.Read(); float x = reader.GetSingle();
            reader.Read(); float y = reader.GetSingle();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { }

            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteEndArray();
        }
    }

    public class JsonVector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) 
                throw new JsonException("Expected array for Vector3");

            reader.Read(); float x = reader.GetSingle();
            reader.Read(); float y = reader.GetSingle();
            reader.Read(); float z = reader.GetSingle();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { }

            return new Vector3(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteEndArray();
        }
    }

    public class JsonVector4Converter : JsonConverter<Vector4>
    {
        public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) 
                throw new JsonException("Expected array for Vector4");

            reader.Read(); float x = reader.GetSingle();
            reader.Read(); float y = reader.GetSingle();
            reader.Read(); float z = reader.GetSingle();
            reader.Read(); float w = reader.GetSingle();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { }

            return new Vector4(x, y, z, w);
        }

        public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteNumberValue(value.W);
            writer.WriteEndArray();
        }
    }
}