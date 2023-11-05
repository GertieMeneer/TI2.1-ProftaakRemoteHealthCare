using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareServer.ClientConnection.Commands.StatusMessage;
using TI2._1_HealthCareServer.ClientInformation;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Sessions
{
    public class SessionFetchInactiveHandler
    {
        public SessionFetchInactiveHandler()
        {

        }

        public void Handle()
        {

            // List<HistoricSession> historicSessions = ClientDataManager.GetHistoricSessions();
            // string historicSessionsJson = JsonConvert.SerializeObject(historicSessions);
            //
            // //convert json to bytes
            // byte[] jsonDataBytes = Encoding.ASCII.GetBytes(historicSessionsJson);
            // int dataLength = jsonDataBytes.Length;
            //
            // byte[] lengthBytes = BitConverter.GetBytes((uint)dataLength);
            // _stream.Write(lengthBytes, 0, lengthBytes.Length);
            //
            // //send byte array
            // _stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
        }
    }
}
