using MotionDetectorApi;
using MotionDetectorApi.Helpers;

namespace MotionDetectorApiTests
{
    [TestClass]
    public class Environment
    {
        // !! if you're thinking of changing this to suit your local setup, please don't !!
        // add a file called "connectionString.txt" in MotionDetectorApi\MotionDetectorApiTests\bin\Debug\net6.0 instead
        // put your desired connection string in that file, it will be git ignored since bin is gitignored
        // it might disappear and you will have to add it again if you do a clean or delete all the built files
        // but this is the best way we have of doing this right now
        private static string connectionString = "postgres://postgres:postgres@localhost:5432/motion-detector-test"; // don't change to suit your local setup

        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            if (File.Exists("connectionString.txt")) // if you add a file called "connectionString.txt" in MotionDetectorApi\MotionDetectorApiTests\bin\Debug\net6.0 it will use that
                connectionString = File.ReadAllText("connectionString.txt");

            System.Environment.SetEnvironmentVariable("DATABASE_URL", connectionString);

            Program.SetupDatabase();
        }

        [TestMethod]
        public void AssertTestEnvironment()
        {
            Assert.AreEqual(connectionString, EnvironmentHelper.GetEnvironmentVariable("DATABASE_URL"));
        }
    }
}
