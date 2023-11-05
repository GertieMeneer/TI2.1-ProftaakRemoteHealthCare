using TI2._1_HealthCareServer.ClientConnection.Commands.DataSend.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.Commands.DataSend
{
    public class DataSend : IClientCommand
    {
        public string Command = "data/send";
        public DataSendAttribute Data;
    }
}
