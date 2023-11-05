using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.SessionStop;
using TI2._1_HealthCareServer.ClientConnection.Commands.SessionStop.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Sessions
{
    public class SessionStopHandler
    {
        private JObject _jObject;
        private ClientDataManager _cdm;
        public SessionStopHandler(JObject jObject, ClientDataManager cdm)
        {
            _jObject = jObject;
            _cdm = cdm;
        }

        public void Handle()
        {
            var patientIdStop = _jObject["data"]["patientid"].ToString();

            Communicator.WriteToClient(
                new SessionStop() { Data = new SessionStopDataAttribute() { PatientId = patientIdStop } },
                ClientInfoManager.FindBikeWithPatientId(patientIdStop));

            _cdm.Stop();

        }
    }
}
