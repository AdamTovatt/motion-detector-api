namespace MotionDetectorApi.Authorization
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private static string? allowedApiKey;

        public bool IsValid(string apiKey)
        {
            if (string.IsNullOrEmpty(allowedApiKey))
                allowedApiKey = Environment.GetEnvironmentVariable("API_KEY");

            if (string.IsNullOrEmpty(allowedApiKey))
                throw new Exception("The api is badly configured and is missing api key value in the environment variables");

            return allowedApiKey == apiKey;
        }
    }
}
