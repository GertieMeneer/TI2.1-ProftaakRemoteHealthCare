using TI2._1_HealthCareClient.Commands.SkyBox.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.SkyBox
{
    public class SkyBoxSetTime : IVRCommand
    {
        public string Id { get { return "scene/skybox/settime"; } }
        public SkyBoxSetTimeDataAttribute Data { get; set; }
    }
}
