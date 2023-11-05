using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient.DataAttributes;

namespace TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient
{
    public class ConnectionConnectPatient : IServerCommand
    {
        public string Command => "connection/connect/patient";
        public TypeDataAttribute Data { get; set; }
    }
}