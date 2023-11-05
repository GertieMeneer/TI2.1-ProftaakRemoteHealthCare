using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.Commands.General.DataAttributes;

namespace TI2._1_HealthCareClient.VrConnection.Commands.General
{
    public class TunnelCreate : IVRCommand
    {
        public string Id { get { return "tunnel/create"; } }
        public TunnelCreateDataAttribute Data { get; set; }
    }
}
