using System.Text.Json.Serialization;

namespace MotionDetectorApi.Models
{
    public class RegisterMotionResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        public RegisterMotionResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
