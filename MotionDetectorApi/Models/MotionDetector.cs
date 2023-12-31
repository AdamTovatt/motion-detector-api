﻿using System.Text.Json.Serialization;

namespace MotionDetectorApi.Models
{
    public class MotionDetector
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("lastMotion")]
        public DateTime? LastMotion { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string SecretKey { get; set; }

        public MotionDetector(int id, string name, string secretKey, DateTime? lastMotion)
        {
            Id = id;
            LastMotion = lastMotion;
            Name = name;
            SecretKey = secretKey;
        }
    }
}
