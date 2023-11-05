using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using TI2._1_HealthCareServer.ClientConnection.Commands;
using System.Net.Sockets;
using System.Threading;
using TI2._1_HealthCareServer.ClientInformation;

namespace TI2._1_HealthCareServer.ClientConnection
{
    /// <summary>
    /// Class responsible for reading or sending data from and to clients
    /// </summary>
    public class Communicator
    {
        //Settings for the JsonSerializer
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        /// <summary>
        /// Reading the incoming data
        /// </summary>
        /// <param name="sslStream">SslStream that is being read from</param>
        /// <param name="ch">Clienthandler object that calls this function</param>
        /// <param name="cdm">ClientDataManager object from the clienthandler</param>
        /// <returns>JSON object with data</returns>
        public static JObject Read(SslStream sslStream, ClientHandler ch, ClientDataManager cdm)
        {
            try
            {
                //Create a bufer of size 4 to receive the length of the incoming message
                byte[] responseBuffer = new byte[4];
                int bytesRead = sslStream.Read(responseBuffer, 0, responseBuffer.Length);

                //Reverse the byte array (received message is in big endian order (most significant byte stored at lowest memory address) and convert it to an int)
                responseBuffer.Reverse();
                int responseLength = BitConverter.ToInt32(responseBuffer, 0);

                //Create a buffer to receive message from the _server, read message into it until the entire message is received
                byte[] buffer = new byte[responseLength];
                int totalBytesRead = 0;

                while (totalBytesRead < responseLength)
                {
                    bytesRead = sslStream.Read(buffer, totalBytesRead, responseLength - totalBytesRead);
                    totalBytesRead += bytesRead;
                }

                JObject message =
                    JsonConvert.DeserializeObject<JObject>(Encoding.ASCII.GetString(buffer, 0, responseLength),
                        _serializerSettings);
                //
                // Console.WriteLine("Data received:");
                // Console.WriteLine(message);
                // Console.WriteLine("\n");

                return message;
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong reading from the client.\nMaybe the client lost connection?");
                if (Dokter.GetDokter() == sslStream)
                {
                    //Remove the dokter if it's really him
                    Dokter.RemoveDokter();
                }
                else
                {
                    //Remove the connected client (aka bike)
                    ClientInfoManager.RemoveBike(sslStream);
                }

                //Stop the clienthandler thread (because not needed anymore, client is gone)
                ch.Stop();
                //Stop the datamanager thread (because not needed anymore, client is gone)
                cdm.Stop();
            }

            return null;
        }

        /// <summary>
        /// Send data back to the client who sent it
        /// </summary>
        /// <param name="iClientCommand">The data</param>
        /// <param name="sslStream">The client who started sending data</param>
        public static void WriteBack(IClientCommand iClientCommand, SslStream sslStream)
        {
            //Set serializer settings
            _serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _serializerSettings.Formatting = Formatting.Indented;

            byte[] jsonArray =
                Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(iClientCommand, _serializerSettings));
            byte[] length = BitConverter.GetBytes((uint)jsonArray.Length);
            byte[] message = length.Concat(jsonArray).ToArray();

            // Console.WriteLine("Sending message:");
            // Console.WriteLine(JsonConvert.SerializeObject(iClientCommand, _serializerSettings));
            // Console.WriteLine("\n");

            sslStream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Send data to a specific client
        /// </summary>
        /// <param name="iClientCommand">The data</param>
        /// <param name="sslStream">The client who has to receive the data</param>
        public static void WriteToClient(IClientCommand iClientCommand, SslStream sslStream)
        {
            //Set serializer settings
            _serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _serializerSettings.Formatting = Formatting.Indented;

            byte[] jsonArray =
                Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(iClientCommand, _serializerSettings));

            //Get the length of the JSON message and append it to the beginning of message
            byte[] length = BitConverter.GetBytes((uint)jsonArray.Length);
            byte[] message = length.Concat(jsonArray).ToArray();

            // Console.WriteLine("Sending message:");
            // Console.WriteLine(JsonConvert.SerializeObject(iClientCommand, _serializerSettings));
            // Console.WriteLine("\n");

            sslStream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Send data to the dokter
        /// </summary>
        /// <param name="iClientCommand">The data</param>
        
        public static void WriteToDokter(IClientCommand iClientCommand)
        {
            Thread.Sleep(100);
            //Set serializer settings
            _serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _serializerSettings.Formatting = Formatting.Indented;

            byte[] jsonArray =
                Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(iClientCommand, _serializerSettings));

            //Get the length of the JSON message and append it to the beginning of message
            byte[] length = BitConverter.GetBytes((uint)jsonArray.Length);
            byte[] message = length.Concat(jsonArray).ToArray();

            // Console.WriteLine("Sending message:");
            // Console.WriteLine(JsonConvert.SerializeObject(iClientCommand, _serializerSettings));
            // Console.WriteLine("\n");

            Dokter.GetDokter().Write(message, 0, message.Length);
        }
    }
}