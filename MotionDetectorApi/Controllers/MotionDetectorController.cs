using Microsoft.AspNetCore.Mvc;
using MotionDetectorApi.Managers;
using MotionDetectorApi.Models;
using MotionDetectorApi.RateLimiting;
using System.Net;

namespace MotionDetectorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
                return new ObjectResult(new RegisterMotionResult(false, $"No motion detector with id: {body.Id} found")) { StatusCode = (int)HttpStatusCode.BadRequest };

            if (detector.SecretKey != body.SecretKey)
                return new ObjectResult(new RegisterMotionResult(false, "Secret key is invalid")) { StatusCode = (int)HttpStatusCode.BadRequest };

            detector.LastMotion = DateTime.Now;

            return new ObjectResult(new RegisterMotionResult(true, "Ok")) { StatusCode = (int)HttpStatusCode.OK };
        }

        [HttpPost("create-new")]
        [Limit(MaxRequests = 50, TimeWindow = 10)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ObjectResult> CreateNew()
        {
            MotionDetector detector = await MotionDetectorManager.Instance.CreateNew("Unnamed motion detector");

            return new ObjectResult(new CreateMotionDetectorResult(detector.Id, detector.SecretKey)) { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}