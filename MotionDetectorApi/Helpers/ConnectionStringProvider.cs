using WebApiUtilities.Helpers;

namespace MotionDetectorApi.Helpers
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString()
        {
            return ConnectionStringHelper.GetConnectionStringFromUrl(EnvironmentHelper.GetEnvironmentVariable("DATABASE_URL"), SslMode.Prefer);
        }
    }
}
