using System;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using T2._1_HealthCareDokter.Backend.Commands;
using T2._1_HealthCareDokter.Backend.Commands.ConnectionConnectDokter;
using T2._1_HealthCareDokter.Backend.Commands.ConnectionConnectDokter.DataAttributes;
using T2._1_HealthCareDokter.GUI.Popups;
using T2._1_HealthCareDokter.GUI.UserControls;

namespace T2._1_HealthCareDokter.Backend.ServerConnection
{
    public static class ServerConnection
    {
        private static TcpClient Client;
        private static NetworkStream Stream;
        private static SslStream SslStream;

        private static bool _debugMode = true;

        private static MainWindow mainWindow { get; set; }

        static ServerConnection()
        {
        }

        public static void Connect(string ServerIp, int ServerPort)
        {
            try
            {
                Client = new TcpClient(ServerIp, ServerPort);
                Stream = Client.GetStream();

                // Set up the SSL stream using your client certificate
                X509Certificate2 clientCertificate =
                    new X509Certificate2("RootCA.pfx", "password");
                SslStream = new SslStream(Stream, false, ValidateServerCertificate);
                SslStream.AuthenticateAsClient(ServerIp, new X509Certificate2Collection(clientCertificate),
                    SslProtocols.Tls12, false);

                Thread receiverThread = new Thread(() => HandleReceivedData());
                receiverThread.Start();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void Write(IDokterCommand command)
        {
            // Set the serializer settings. JSON formatting should be maintained, while the entire structure should be in lowercase
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            // Format the command to a JSON array using the settings above
            byte[] jsonArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(command, serializerSettings));

            // Get the length of the JSON message and append it to the beginning of message
            byte[] length = BitConverter.GetBytes((uint)jsonArray.Length);
            byte[] message = length.Concat(jsonArray).ToArray();

            // Send the message to the server

            if (_debugMode)
            {
                Console.WriteLine("Sending message:");
                Console.WriteLine(JsonConvert.SerializeObject(command, serializerSettings));
                Console.WriteLine("\n");
            }
            else
            {
                JObject jsonObject = JObject.Parse(JsonConvert.SerializeObject(command, serializerSettings));

                if ((jsonObject["id"].ToString() != "session/list") && (jsonObject["id"].ToString() != "tunnel/create"))
                {
                    Console.WriteLine(jsonObject["data"]["data"]["id"]);
                }
            }

            SslStream.Write(message, 0, message.Length);
        }

        private static void HandleReceivedData()
        {
            while (true)
            {
                var jObject = Read();

                var command = jObject["command"].ToString();
                switch (command)
                {
                    case "data/send":
                        var heartrate = jObject["data"]["heartrate"].ToString();
                        var distance = jObject["data"]["distance"].ToString();
                        var speed = jObject["data"]["speed"].ToString();

                        mainWindow.AddValues(heartrate,distance,speed);
                      

                        break;

                    case "connection/connect/dokter":

                        LoginWindow.receiveddata = true;
                        var statusConnect = jObject["data"]["status"].ToString();

                        if (statusConnect.Equals("ok"))
                        {
                           
                            LoginWindow.loggedin = true;
                        }
                        else
                        {
                         
                            LoginWindow.loggedin = false;
                        }
                        break;

                    default:
                        var status = jObject["data"]["status"].ToString();
                        if (!status.Equals("ok"))
                        {
                            var error = jObject["data"]["error"].ToString();
                            new ErrorPopup(error).Show();
                        }

                        break;
                }
            }
        }

        private static JObject Read()
        {
            // Create a bufer of size 4 to receive the length of the incoming message
            byte[] responseBuffer = new byte[4];
            int bytesRead = SslStream.Read(responseBuffer, 0, responseBuffer.Length);

            // Reverse the byte array (since the received message is in big endian and convert it to an int)
            responseBuffer.Reverse();
            int responseLength = BitConverter.ToInt32(responseBuffer, 0);

            // Create a buffer to receive message from the server an read message into it until the entire message is received
            byte[] buffer = new byte[responseLength];
           

            for (int i = 0; i < responseLength; i++)
            {
                int recievedValue = SslStream.ReadByte();
                if (recievedValue == -1) break;
                buffer[i] = (byte)recievedValue;
            }

            // Set the serializer settings. JSON formatting should be maintained, while the entire structure should be in lowercase
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            // Deserialize using the settings created above
            JObject message =
                JsonConvert.DeserializeObject<JObject>(Encoding.ASCII.GetString(buffer, 0, responseLength),
                    serializerSettings);

            if (true)
            {
                Console.WriteLine("Transform received:");
                Console.WriteLine(message);
                Console.WriteLine("\n");
            }

            return message;
        }

        public static void bindWindow(MainWindow window)
        {
            mainWindow = window;
            
        }
    }
}