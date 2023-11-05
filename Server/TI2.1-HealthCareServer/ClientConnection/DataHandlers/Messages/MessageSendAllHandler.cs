using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendAll;
using TI2._1_HealthCareServer.ClientConnection.Commands.MessageSendAll.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Messages
{
    public class MessageSendAllHandler
    {
        private JObject _jObject;
        public MessageSendAllHandler(JObject jObject)
        {
            _jObject = jObject;
        }

        public void Handle()
        {
            string messageToAll = _jObject["data"]["message"].ToString();
            string senderToAll = _jObject["data"]["sender"].ToString();
            
            //ga door de lijst met alle verbonden fietsen waar patienten op zitten en stuur het bericht
            foreach (var client in ClientInfoManager.GetBikesWithPatients())
            {
                Communicator.WriteToClient
                    (new MessageSendAll() { Data = new MessageSendAllData(){ Message = messageToAll, Sender = senderToAll } }, client.Stream);
            }
        }
    }
}
