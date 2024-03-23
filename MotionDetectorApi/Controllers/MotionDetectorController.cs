using HueApi;
using HueApi.Models;
using HueApi.Models.Clip;
using HueApi.Models.Exceptions;
using HueApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using MotionDetectorApi.Authorization;
using MotionDetectorApi.Helpers;
using MotionDetectorApi.Managers;
using MotionDetectorApi.Models;
using MotionDetectorApi.RateLimiting;
using MotionDetectorApi.Repositories;
using System.Net;

namespace MotionDetectorApi.Controllers
{
    [ApiController]
    [Route("")]
    public class MotionDetectorController : ControllerBase
    {
        [HttpGet("get-list")]
        [Limit(MaxRequests = 50, TimeWindow = 10)]
        [ProducesResponseType(typeof(List<MotionDetector>), (int)HttpStatusCode.OK)]
        public async Task<ObjectResult> GetList()
        {
            List<MotionDetector> list = await MotionDetectorManager.Instance.GetListAsync();
            return new ObjectResult(list) { StatusCode = (int)HttpStatusCode.OK };
        }

        [HttpPost("register-motion")]
        [Limit(MaxRequests = 50, TimeWindow = 10)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ObjectResult> RegisterMotion([FromBody] RegisterMotionBody body)
        {
            if (body.SecretKey == null)
                return new ObjectResult(new RegisterMotionResult(false, "Secret key is required")) { StatusCode = (int)HttpStatusCode.BadRequest };

            if (body.Id == 0)
                return new ObjectResult(new RegisterMotionResult(false, "Id is required")) { StatusCode = (int)HttpStatusCode.BadRequest };

            MotionDetector? detector = await MotionDetectorManager.Instance.GetAsync(body.Id);

            if (detector == null)
                return new ObjectResult(new RegisterMotionResult(false, $"No motion detector with id: {body.Id} found")) { StatusCode = (int)HttpStatusCode.Forbidden };

            if (detector.SecretKey != body.SecretKey)
                return new ObjectResult(new RegisterMotionResult(false, "Secret key is invalid")) { StatusCode = (int)HttpStatusCode.Forbidden };

            bool success = await MotionDetectorManager.Instance.RegisterMotionAsync(body.Id, body.SecretKey);

            if (success)
            {
                try
                {
                    LightsManager hue = new LightsManager(await HueApiHelper.GetLocalHueApiAsync());
                    await hue.FetchLightsStatusAsync();
                    await hue.HandleMotionDetected(detector.LastMotion ?? DateTime.Now);
                }
                catch { }
            }

            return new ObjectResult(new RegisterMotionResult(success, success ? "Ok" : "Unknown error")) { StatusCode = (int)HttpStatusCode.OK };
        }

        [RequireApiKey]
        [HttpPost("create-new")]
        [Limit(MaxRequests = 50, TimeWindow = 10)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ObjectResult> CreateNew()
        {
            MotionDetector detector = await MotionDetectorManager.Instance.CreateNew("Unnamed motion detector");

            return new ObjectResult(new CreateMotionDetectorResult(detector.Id, detector.SecretKey)) { StatusCode = (int)HttpStatusCode.OK };
        }

        [RequireApiKey]
        [HttpPost("setup-hue")]
        [Limit(MaxRequests = 50, TimeWindow = 10)]
        [ProducesResponseType(typeof(HueCredentials), (int)HttpStatusCode.OK)]
        public async Task<ObjectResult> SetupHue(string ipAddress)
        {
            HueCredentials? credentials;

            try
            {
                RegisterEntertainmentResult? registerResult = await LocalHueApi.RegisterAsync(ipAddress, "motion-detector", "raspberry-pi", true);

                if (registerResult == null || registerResult.Username == null || registerResult.StreamingClientKey == null || registerResult.Ip == null)
                    return new ObjectResult("Register result was null but no exception was thrown");

                credentials = new HueCredentials(0, registerResult.StreamingClientKey, registerResult.Ip, registerResult.Username);
                await HueCredentialsRepository.Instance.InsertAsync(credentials);
            }
            catch (HueException exception)
            {
                return new ObjectResult(exception.Message) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return new ObjectResult(credentials) { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}