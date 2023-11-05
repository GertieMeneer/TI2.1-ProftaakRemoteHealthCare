using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Data
{
    public class DataFetchHandler
    {
        private JObject _jObject;

        public DataFetchHandler(JObject jObject)
        {
            _jObject = jObject;
        }

        public void Handle()
        {
            var dataFetchPatientId = _jObject["data"]["patient-id"].ToString();
            var dataFetchSessionId = _jObject["data"]["session-id"].ToString();
            var date = _jObject["data"]["date"].ToString();
            string[] files =
                Directory.GetFiles(
                    $"{Environment.CurrentDirectory}\\data_{date}_{dataFetchPatientId}_{dataFetchSessionId}.csv");
            if (files.Length > 0)
            {
                //eerst ok terug sturen naar dokter > dan dokter voorbereiden op binary meuk
                // WriteToDokter(new StatusMessage() { Command = command, Data = new StatusOk() });
                // BinaryWriter binaryWriter = new BinaryWriter(_client.GetStream());
                string fileToSend = files[0];
                string fileContent = File.ReadAllText(fileToSend);

                // binaryWriter.Write(fileContent);
                // binaryWriter.Close();
            }
            else
            {
                // WriteToDokter(new StatusMessage()
                // {
                // Command = command,
                // Data = new StatusError()
                // { Error = "The requested information was not found on the _server." }
                // });
            }
        }
    }
}