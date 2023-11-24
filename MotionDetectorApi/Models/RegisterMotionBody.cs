using System.Text.Json.Serialization;

namespace MotionDetectorApi.Models
{
    public class RegisterMotionBody
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("secretKey")]
        public string? SecretKey { get; set; }
    }
}
