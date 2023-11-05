using TI2._1_HealthCareServer.ClientConnection.Commands.SessionStop.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.Commands.SessionStop
{
    public class SessionStop : IClientCommand
    {
        public string Command = "session/stop";
        public SessionStopDataAttribute Data;
    }
}
