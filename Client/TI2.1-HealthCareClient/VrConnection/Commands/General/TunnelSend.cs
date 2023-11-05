using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VrConnectionCommands.General.DataAttributes;

namespace TI2._1_HealthCareClient.VrConnection.Commands.General
{
    public class TunnelSend : IVRCommand
    {
        public string Id { get { return "tunnel/send"; } }

        public TunnelSendDataAttribute Data { get; set; }
    }
}
