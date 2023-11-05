using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Node.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Node
{
    public class SceneNodeUpdate : IVRCommand
    {
        public string Id { get { return "scene/node/update"; } }
        public SceneNodeUpdateDataAttribute Data { get; set; }
    }
}
