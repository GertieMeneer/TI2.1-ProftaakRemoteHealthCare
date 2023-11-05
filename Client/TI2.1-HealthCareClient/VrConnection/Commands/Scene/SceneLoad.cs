using TI2._1_HealthCareClient.Commands.Scene.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Scene
{
    public class SceneLoad : IVRCommand
    {
        public string Id { get { return "scene/load"; } }
        public SceneLoadDataAttribute Data { get; set; }
    }
}
