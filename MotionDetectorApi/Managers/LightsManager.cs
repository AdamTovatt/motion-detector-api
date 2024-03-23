using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;

namespace MotionDetectorApi.Managers
{
    public class LightsManager
    {
        private LocalHueApi hue;

        public List<Light> Lights { get; set; }

        public LightsManager(LocalHueApi hue)
        {
            this.hue = hue;
            Lights = new List<Light>();
        }

        public async Task FetchLightsStatusAsync()
        {
            Lights.Clear();

            HueResponse<Light> lights = await hue.GetLightsAsync();

            lights.Data.ForEach(light => { Lights.Add(light); });
        }

        public bool GetLightsAreOn()
        {
            return Lights.All(x => x.On.IsOn);
        }

        public async Task TurnOnAllLightsAsync()
        {
            UpdateLight req = new UpdateLight().TurnOn();

            foreach (Light light in Lights)
                await hue.UpdateLightAsync(light.Id, req);
        }

        public async Task TurnOffAllLightsAsync()
        {
            UpdateLight req = new UpdateLight().TurnOff();

            foreach (Light light in Lights)
                await hue.UpdateLightAsync(light.Id, req);
        }

        public async Task HandleMotionDetected(DateTime lastPreviousMotion)
        {
            if (!GetLightsAreOn()) // lights are turned off
            {
                double hoursSinceLastMotion = (DateTime.Now - lastPreviousMotion).TotalHours;

                if(hoursSinceLastMotion > 4)
                {
                    await TurnOnAllLightsAsync();
                }
            }
        }
    }
}
