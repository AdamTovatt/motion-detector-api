namespace MotionDetectorApi.Authorization
{
    public interface IApiKeyValidator
    {
        bool IsValid(string apiKey);
    }
}
