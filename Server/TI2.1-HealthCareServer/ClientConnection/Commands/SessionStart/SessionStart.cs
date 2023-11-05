using TI2._1_HealthCareServer.ClientConnection.Commands.SessionStart.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.Commands.SessionStart
{
    public class SessionStart : IClientCommand
    {
        public string Command = "session/start";
        public SessionStartDataAttribute Data;
    }
}
