using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.ServerConnection.Commands.SessionStop.DataAttributes;

namespace TI2._1_HealthCareClient.ServerConnection.Commands.SessionStop
{
    public class SessionStop : IServerCommand
    {
        public string Command = "session/stop";
        public SessionStopDataAttributes data;
    }
}
