using TI2._1_HealthCareClient.Commands;

namespace TI2._1_HealthCareClient.VrConnectionCommands.General.DataAttributes
{
    public class TunnelSendDataAttribute
    {
        public string Dest { get; set; }
        public IVRCommand Data { get; set; }
    }
}