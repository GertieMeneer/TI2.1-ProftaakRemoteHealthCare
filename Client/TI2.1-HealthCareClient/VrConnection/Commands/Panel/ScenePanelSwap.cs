using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Panel
{
    public class ScenePanelSwap : IVRCommand
    {
        public string Id { get { return "scene/panel/swap"; } }
        public ScenePanelSwapDataAttribute Data { get; set; }
    }
}
