using Newtonsoft.Json;

namespace WA_Kingpos.Data
{
    public class CustomJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // Adjust this to target specific types you want to handle
            return objectType.IsPrimitive || objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            // Attempt to deserialize simple types directly, defer to the default serializer for complex types
            try
            {
                return Convert.ChangeType(reader.Value, objectType);
            }
            catch
            {
                return serializer.Deserialize(reader, objectType);
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Type valueType = value.GetType();

            if (valueType.IsPrimitive || valueType == typeof(string))
            {
                writer.WriteValue(value);
            }
            else
            {
                // Use default serialization for complex objects
                serializer.Serialize(writer, value);
            }
        }
    }
}
