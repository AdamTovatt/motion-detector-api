using System.Text.Json.Serialization;

namespace MotionDetectorApi.Models
{
    public class CreateMotionDetectorResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("secretKey")]
        public string SecretKey { get; set; }

        public CreateMotionDetectorResult(int id, string secretKey)
        {
            Id = id;
            SecretKey = secretKey;
        }
    }
}
