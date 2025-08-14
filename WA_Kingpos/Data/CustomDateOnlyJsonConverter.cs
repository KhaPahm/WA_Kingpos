using Newtonsoft.Json;
using System.Globalization;

namespace WA_Kingpos.Data
{

    /// <summary>
    /// json cast từ datetime hoặc object, xóa time để thành string date dd/MM/yyyy
    /// </summary>
    public class CustomDateOnlyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }
            return Convert.ToDateTime(reader.Value, CultureInfo.GetCultureInfo("en-GB")).ToString("dd/MM/yyyy");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
