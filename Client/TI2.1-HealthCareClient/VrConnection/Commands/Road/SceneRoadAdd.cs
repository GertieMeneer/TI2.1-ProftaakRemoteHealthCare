using TI2._1_HealthCareClient.Commands.Road.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Road
{
    public class SceneRoadAdd : IVRCommand
    {
        public string Id { get { return "scene/road/add"; } }
        public SceneRoadAddDataAttribute Data { get; set; }
    }
}
