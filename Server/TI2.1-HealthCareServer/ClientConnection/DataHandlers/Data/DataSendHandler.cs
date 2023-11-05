using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.DataSend;
using TI2._1_HealthCareServer.ClientConnection.Commands.DataSend.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Data
{
    public class DataSendHandler
    {
        private JObject _jObject;
        private ClientDataManager _cdm;

        public DataSendHandler(JObject jObject, ClientDataManager cdm)
        {
            _jObject = jObject;
            _cdm = cdm;
        }

        public void Handle()
        {
            string heartrate = "";
            string speed = "";
            string distance = "";

            if (_jObject["data"]["heartrate"] != null)
            {
                heartrate = _jObject["data"]["heartrate"].ToString();
            }
            if (_jObject["data"]["speed"] != null)
            {
                speed = _jObject["data"]["speed"].ToString();
            }
            if (_jObject["data"]["distance"] != null)
            {
                distance = _jObject["data"]["distance"].ToString();
            }

            // Console.WriteLine($"{heartrate}, {speed}, {distance}");
            _cdm.SaveData($"{heartrate}, {speed}, {distance}");
            
            Communicator.WriteToDokter(new DataSend()
            {
                Data = new DataSendAttribute() { distance = distance, heartrate = heartrate, speed = speed }
            });
        }
    }
}