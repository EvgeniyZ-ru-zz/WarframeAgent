using System;
using Newtonsoft.Json;

namespace Core.Converters
{
    public class IgnoreNonObjectConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(T);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
                return serializer.Deserialize<T>(reader);
            serializer.Deserialize<object>(reader);
            return default(T);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);
    }
}
