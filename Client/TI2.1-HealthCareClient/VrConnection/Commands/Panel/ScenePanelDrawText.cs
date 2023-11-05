using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Panel
{
    public class ScenePanelDrawText : IVRCommand
    {
        public string Id { get { return "scene/panel/drawtext"; } }   
        public ScenePanelDrawTextDataAttribute Data { get; set; }
    }
}
