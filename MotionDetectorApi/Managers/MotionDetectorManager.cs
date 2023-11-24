using MotionDetectorApi.Helpers;
using MotionDetectorApi.Models;

namespace MotionDetectorApi.Managers
{
    public class MotionDetectorManager
    {
        public static MotionDetectorManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MotionDetectorManager();
                return _instance;
            }
        }

        private static MotionDetectorManager? _instance;

        private Dictionary<int, MotionDetector> motionDetectors;

        public MotionDetectorManager()
        {
            motionDetectors = new Dictionary<int, MotionDetector>();
        }

        public MotionDetector? Get(int id)
        {
            if (motionDetectors.TryGetValue(id, out MotionDetector? result)) return result;
            return null;
        }

        public List<MotionDetector> GetList()
        {
            return motionDetectors.Values.ToList();
        }

        public MotionDetector CreateNew(string name)
        {
            int newId = 1;

            if (motionDetectors.Count > 0)
                newId = motionDetectors.Keys.Max() + 1;

            MotionDetector detector = new MotionDetector(newId, null, name, StringIdProvider.Instance.GenerateId(16));
            motionDetectors.Add(newId, detector);

            return detector;
        }
    }
}
