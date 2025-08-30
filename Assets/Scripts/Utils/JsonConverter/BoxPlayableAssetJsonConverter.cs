using System;
using Kirara.TimelineAction;
using Newtonsoft.Json;

namespace Kirara
{
    public class BoxPlayableAssetJsonConverter : JsonConverter<BoxPlayableAsset>
    {
        public override void WriteJson(JsonWriter writer, BoxPlayableAsset value, JsonSerializer serializer)
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
            writer.WritePropertyName("setRot");
            serializer.Serialize(writer, value.setRot);
            writer.WritePropertyName("rotValue");
            serializer.Serialize(writer, value.rotValue);
            writer.WritePropertyName("rotMaxValue");
            serializer.Serialize(writer, value.rotMaxValue);
            writer.WritePropertyName("hitGatherDist");
            serializer.Serialize(writer, value.hitGatherDist);
            writer.WriteEndObject();
        }

        public override BoxPlayableAsset ReadJson(JsonReader reader, Type objectType, BoxPlayableAsset existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return null;
        }
    }
}