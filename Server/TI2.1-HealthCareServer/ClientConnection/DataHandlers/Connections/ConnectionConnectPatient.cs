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
    public class ConnectionConnectPatient
    {
        private JObject _jObject;
        private SslStream _stream;

        public ConnectionConnectPatient(JObject jObject, SslStream stream)
        {
            _jObject = jObject;
            _stream = stream;
        }

        public void Handle()
        {
            var patientidconnect = _jObject["data"]["patientid"].ToString();
            var patientname = _jObject["data"]["patientname"].ToString();

            ClientInfoManager.AddPatient(patientidconnect, patientname);
            ClientInfoManager.SetPatientForBike(_stream, patientidconnect, true);
        }
    }
}