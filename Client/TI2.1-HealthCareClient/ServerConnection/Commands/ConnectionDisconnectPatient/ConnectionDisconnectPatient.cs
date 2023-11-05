using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectPatient.DataAttributes;

namespace TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectPatient
{
    public class ConnectionDisconnectPatient : IServerCommand
    {
        public string command = "connection/disconnect/patient";
        public InfoDataAttribute data;
    }
}
