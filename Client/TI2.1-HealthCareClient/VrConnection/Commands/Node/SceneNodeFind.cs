using TI2._1_HealthCareClient.Commands.Node.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Node
{
    public class SceneNodeFind : IVRCommand
    {
        public string Id { get { return "scene/node/find"; } }
        public SceneNodeFindDataAttribute Data { get; set; }
    }
}
