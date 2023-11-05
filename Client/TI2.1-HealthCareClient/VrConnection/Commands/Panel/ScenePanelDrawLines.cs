using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Panel
{
    public class ScenePanelDrawLines : IVRCommand
    {
        public string Id { get { return "scene/panel/drawlines"; } }
        public ScenePanelDrawLinesDataAttribute Data { get; set; }
    }
}
