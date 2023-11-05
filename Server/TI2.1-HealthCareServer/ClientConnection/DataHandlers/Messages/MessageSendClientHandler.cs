using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendClient;
using TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendClient.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Messages
{
    public class MessageSendClientHandler
    {
        private JObject _jObject;

        public MessageSendClientHandler(JObject jObject)
        {
            _jObject = jObject;
        }

        public void Handle()
        {
            string messageToClient = _jObject["data"]["message"].ToString();
            string patientId = _jObject["data"]["patientid"].ToString();
            string sender = _jObject["data"]["sender"].ToString();

            Communicator.WriteToClient(
                new MessageSendClient()
                    { Data = new MessageSendClientData() { Message = messageToClient, Sender = sender } },
                ClientInfoManager
                    .FindBikeWithPatientId(patientId));
        }
    }
}