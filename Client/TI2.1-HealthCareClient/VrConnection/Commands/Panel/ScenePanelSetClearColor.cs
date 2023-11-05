using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Panel
{
    public class ScenePanelSetClearColor : IVRCommand
    {
        public string Id { get { return "scene/panel/setclearcolor"; } }
        public ScenePanelSetClearColorDataAttribute Data { get; set; }
    }
}
