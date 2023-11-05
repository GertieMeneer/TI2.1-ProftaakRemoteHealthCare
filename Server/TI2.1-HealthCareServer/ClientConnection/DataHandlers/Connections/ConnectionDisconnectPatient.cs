using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Connections
{
    public class ConnectionDisconnectPatient
    {
        private JObject _jObject;
        private SslStream _stream;

        public ConnectionDisconnectPatient(JObject jObject, SslStream stream)
        {
            _jObject = jObject;
            _stream = stream;
        }

        public void Handle()
        {
            var patientiddisconnect = _jObject["data"]["patientid"].ToString();

            ClientInfoManager.SetPatientForBike(_stream, patientiddisconnect, false);
            ClientInfoManager.RemovePatient(patientiddisconnect);
        }
    }
}