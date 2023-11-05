using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.ServerConnection.Commands.SessionStart.DataAttributes;

namespace TI2._1_HealthCareClient.ServerConnection.Commands.SessionStart
{
    public class SessionStart : IServerCommand
    {
        public string Command = "session/start";
        public PatientIdDataAttribute data;
    }
}
