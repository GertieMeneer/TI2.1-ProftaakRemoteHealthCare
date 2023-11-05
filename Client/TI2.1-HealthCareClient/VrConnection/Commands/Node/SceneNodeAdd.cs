using TI2._1_HealthCareClient.Commands.Node.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Node
{
    public class SceneNodeAdd : IVRCommand
    {
        public string Id { get { return "scene/node/add"; } }
        public SceneNodeAddDataAttribute Data { get; set; }
    }
}
