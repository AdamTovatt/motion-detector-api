using Microsoft.AspNetCore.Mvc;

namespace MotionDetectorApi.Authorization
{
    public class RequireApiKeyAttribute : ServiceFilterAttribute
    {
        public RequireApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter)) { }
    }
}
