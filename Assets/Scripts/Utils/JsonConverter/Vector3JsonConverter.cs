using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Kirara
{
    public class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            serializer.Serialize(writer, value.x);
            writer.WritePropertyName("y");
            serializer.Serialize(writer, value.y);
            writer.WritePropertyName("z");
            serializer.Serialize(writer, value.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return new Vector3(
                serializer.Deserialize<float>(reader),
                serializer.Deserialize<float>(reader),
                serializer.Deserialize<float>(reader));
        }
    }
}