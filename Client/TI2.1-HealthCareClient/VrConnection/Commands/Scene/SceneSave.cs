using TI2._1_HealthCareClient.Commands.Scene.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Scene
{
    public class SceneSave : IVRCommand
    {
        public string Id { get { return "scene/save"; } }
        public SceneSaveDataAttribute Data { get; set; }
    }
}
