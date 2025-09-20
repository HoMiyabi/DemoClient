using System;
using Kirara.TimelineAction;
using Newtonsoft.Json;

namespace Kirara
{
    public class BoxNotifyStateJsonConverter : JsonConverter<BoxNotifyState>
    {
        public override void WriteJson(JsonWriter writer, BoxNotifyState value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("start");
            serializer.Serialize(writer, value.start);
            writer.WritePropertyName("length");
            serializer.Serialize(writer, value.length);
            writer.WritePropertyName("boxType");
            serializer.Serialize(writer, value.boxType);
            writer.WritePropertyName("boxShape");
            serializer.Serialize(writer, value.boxShape);
            writer.WritePropertyName("center");
            serializer.Serialize(writer, value.center);
            writer.WritePropertyName("radius");
            serializer.Serialize(writer, value.radius);
            writer.WritePropertyName("size");
            serializer.Serialize(writer, value.size);
            writer.WritePropertyName("hitStrength");
            serializer.Serialize(writer, value.hitStrength);
            writer.WritePropertyName("hitId");
            serializer.Serialize(writer, value.hitId);
            writer.WritePropertyName("setParticleRot");
            serializer.Serialize(writer, value.setParticleRot);
            writer.WritePropertyName("rotValue");
            serializer.Serialize(writer, value.rotValue);
            writer.WritePropertyName("rotMaxValue");
            serializer.Serialize(writer, value.rotMaxValue);
            writer.WritePropertyName("hitGatherDist");
            serializer.Serialize(writer, value.hitGatherDist);
            writer.WriteEndObject();
        }

        public override BoxNotifyState ReadJson(JsonReader reader, Type objectType, BoxNotifyState existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return null;
        }
    }
}