using TI2._1_HealthCareClient.Commands.Node.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Node
{
    public class SceneNodeAddLayer : IVRCommand
    {
        public string Id { get { return "scene/node/addlayer"; } }
        public SceneNodeAddLayerDataAttribute Data { get; set; }
    }
}
