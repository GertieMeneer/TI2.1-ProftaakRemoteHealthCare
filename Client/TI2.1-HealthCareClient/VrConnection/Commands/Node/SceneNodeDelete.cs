using TI2._1_HealthCareClient.Commands.Node.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Node
{
    public class SceneNodeDelete : IVRCommand
    {
        public string Id { get { return "scene/node/delete"; } }
        public SceneNodeDeleteDataAttribute Data { get; set; }
    }
}
