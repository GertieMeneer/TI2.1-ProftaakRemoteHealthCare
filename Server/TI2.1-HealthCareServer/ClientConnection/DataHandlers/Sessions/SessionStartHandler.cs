using System;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.SessionStart.DataAttributes;
using TI2._1_HealthCareServer.ClientConnection.Commands.SessionStart;
using TI2._1_HealthCareServer.ClientConnection.Commands.StatusMessage;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Sessions
{
    public class SessionStartHandler
    {
        private JObject _jObject;
        private ClientDataManager _cdm;

        public SessionStartHandler(JObject jObject, ClientDataManager cdm)
        {
            _jObject = jObject;
            _cdm = cdm;
        }

        public void Handle()
        {
            //send to _client that session has to start
            var patientIdStart = _jObject["data"]["patientid"].ToString();
            try
            {
                //create new file for patient with patientid

                _cdm.CreateFile(patientIdStart);

                Communicator.WriteToClient(
                    new SessionStart() { Data = new SessionStartDataAttribute() { PatientId = patientIdStart } },
                    ClientInfoManager.FindBikeWithPatientId(patientIdStart));

                Communicator.WriteToDokter(new StatusMessage() { Data = new StatusOk() });
            }
            catch (Exception)
            {
                Communicator.WriteToDokter(new StatusMessage()
                    { Data = new StatusError() { Error = "Unable to start session (incorrect patientid maybe?)" } });
            }
        }
    }
}