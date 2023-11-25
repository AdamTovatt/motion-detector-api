using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace MotionDetectorApi.Authorization
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        private const string apiKeyHeaderName = "x-api-key";

        private readonly IApiKeyValidator apiKeyValidator;

        public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
        {
            this.apiKeyValidator = apiKeyValidator;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool allowed = true;

            if (!context.HttpContext.Request.Headers.Any(x => x.Key.ToLower() == "x-api-key")) // does the key header even exist?
            {
                context.Result = new ObjectResult(new { message = "Missing x-api-key header" }) { StatusCode = (int)HttpStatusCode.Unauthorized };
                allowed = false;
            }

            string apiKey = context.HttpContext.Request.Headers[apiKeyHeaderName];

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Result = new ObjectResult(new { message = "Missing value for x-api-key header" }) { StatusCode = (int)HttpStatusCode.Unauthorized };
                allowed = false;
            }

            if (!apiKeyValidator.IsValid(apiKey))
            {
                context.Result = new ObjectResult(new { message = $"The api key {apiKey} is not valid" }) { StatusCode = (int)HttpStatusCode.Unauthorized };
                allowed = false;
            }

            if (!allowed) // add a little delay so they won't think they can brute force the api key
                Thread.Sleep(500);
        }
    }
}
