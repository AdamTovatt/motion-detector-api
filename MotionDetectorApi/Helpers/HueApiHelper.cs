using HueApi;
using MotionDetectorApi.Models;
using MotionDetectorApi.Repositories;

namespace MotionDetectorApi.Helpers
{
    public class HueApiHelper
    {
        public async static Task<LocalHueApi> GetLocalHueApiAsync()
        {
            HueCredentials? hueCredentials = await HueCredentialsRepository.Instance.GetAsync();

            if (hueCredentials == null)
                throw new ArgumentNullException(nameof(hueCredentials), "Missing HUE credentials. Call the setup endpoint first.");

            LocalHueApi localHueApi = new LocalHueApi(hueCredentials.IpAddress, hueCredentials.Username);
            return localHueApi;
        }
    }
}
