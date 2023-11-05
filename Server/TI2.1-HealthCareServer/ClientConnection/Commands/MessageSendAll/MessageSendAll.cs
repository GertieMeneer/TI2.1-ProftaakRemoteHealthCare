using TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendAll.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendAll
{
    public class MessageSendAll : IClientCommand
    {
        public string Command = "message/send/all";
        public MessageSendAllData Data;
    }
}
