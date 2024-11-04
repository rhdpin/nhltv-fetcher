using System.Text.Json;

namespace NhlTvFetcher.Models
{
    public static class JsonDeserializeExtensions
    {
        private static readonly JsonSerializerOptions defaultSerializerSettings = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static T Deserialize<T>(this string json)
        {       
            return JsonSerializer.Deserialize<T>(json, defaultSerializerSettings);
        }
    
        public static T DeserializeCustom<T>(this string json, JsonSerializerOptions settings)
        {
            return JsonSerializer.Deserialize<T>(json, settings);
        }
    }
}
