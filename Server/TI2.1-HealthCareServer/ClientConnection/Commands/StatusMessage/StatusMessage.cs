
namespace TI2._1_HealthCareServer.ClientConnection.Commands.StatusMessage
{
    public class StatusMessage : IClientCommand
    {
        public string Command { get; set; }
        public IStatusMessage Data { get; set; }
    }
}