using Microsoft.AspNetCore.Mvc;
using MotionDetectorApi.Controllers;
using MotionDetectorApi.Managers;
using MotionDetectorApi.Models;

namespace MotionDetectorApiTests
{
    [TestClass]
    public class MotionDetectorControllerTests
    {
        private MotionDetectorController controller = new MotionDetectorController();

        [TestInitialize]
        public async Task BeforeEach()
        {
            await DatabaseHelper.CleanTable("motion_detector");
        }

        [TestMethod]
        public async Task CreateNew()
        {
            ObjectResult objectResult = await controller.CreateNew();

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);

            CreateMotionDetectorResult? result = (CreateMotionDetectorResult)objectResult.Value;

            Assert.IsNotNull(result);

            Assert.IsNotNull(result.SecretKey);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public async Task RegisterMotion()
        {
            await CreateNew();

            MotionDetector detector = (await MotionDetectorManager.Instance.GetListAsync())[0];

            ObjectResult objectResult = await controller.RegisterMotion(new RegisterMotionBody() { Id = detector.Id, SecretKey = detector.SecretKey });

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);

            RegisterMotionResult? result = (RegisterMotionResult)objectResult.Value;

            Assert.IsNotNull(result);

            Assert.IsTrue(result.Success);

            detector = (await MotionDetectorManager.Instance.GetListAsync())[0];

            Assert.IsNotNull(detector.LastMotion);
        }

        [TestMethod]
        public async Task GetList()
        {
            await CreateNew();

            ObjectResult objectResult = await controller.GetList();

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsNotNull(objectResult.Value);

            List<MotionDetector>? result = (List<MotionDetector>)objectResult.Value;

            Assert.IsNotNull(result);

            Assert.AreEqual(1, result.Count);

            MotionDetector detector = result[0];

            Assert.AreEqual(1, detector.Id);
            Assert.AreEqual("Unnamed motion detector", detector.Name);
            Assert.IsNull(detector.LastMotion);
            Assert.IsNotNull(detector.SecretKey);
        }
    }
}