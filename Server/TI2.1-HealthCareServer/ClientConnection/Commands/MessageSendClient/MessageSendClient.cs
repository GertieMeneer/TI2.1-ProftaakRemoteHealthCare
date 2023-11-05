using TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendClient.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendClient
{
    public class MessageSendClient : IClientCommand
    {
        public string Command = "message/send/client";
        public MessageSendClientData Data;
    }
}
