using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TI2._1_HealthCareClient.BLEConnection.Devices;
using TI2._1_HealthCareClient.BLEConnection.Translator;
using TI2._1_HealthCareClient.ServerConnection.Commands;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectBike;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend.DataAttributes;
using TI2._1_HealthCareClient.VRConnection;
using TI2._1_HealthCareClient.VRConnection.Panel.ChatModule;

namespace TI2._1_HealthCareClient.ServerConnection
{
    public delegate void DataReceived(JObject jObject);

    /// <summary>
    /// This class is responsible for managing a connection with the VR Server.
    /// </summary>
    public class ServerClientConnection
    {
        private int _serverPort = 12345;
        private string _serverIP = "127.0.0.1";

        private TcpClient _client;
        private NetworkStream _stream;
        private SslStream _sslStream;

        public bool DebugMode { get; set; } = false;

        private bool _runSendFEData;

        public ServerClientConnection(string ServerIP, int ServerPort)
        {
            _serverIP = ServerIP;
            _serverPort = ServerPort;
        }

        public void Connect()
        {
            try
            {
                _client = new TcpClient(_serverIP, _serverPort);
                _stream = _client.GetStream();

                // Set up the SSL stream using your client certificate
                X509Certificate2 clientCertificate =
                    new X509Certificate2("RootCA.pfx", "password");
                _sslStream = new SslStream(_stream, false, ValidateServerCertificate);
                _sslStream.AuthenticateAsClient(_serverIP, new X509Certificate2Collection(clientCertificate),
                    SslProtocols.Tls12, false);

                Write(new ConnectionConnectBike());

                while (true)
                {
                    var message = Read();
                    if (message != null)
                    {
                        Thread receiverThread = new Thread(() => HandleReceivedData(message));
                        receiverThread.Start();
                    }
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }


        public void Disconnect()
        {
            _sslStream.Close();
        }

        public void Write(IServerCommand command)
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

            if (DebugMode)
            {
                Console.WriteLine("Sending message:");
                Console.WriteLine(JsonConvert.SerializeObject(command, serializerSettings));
                Console.WriteLine("\n");
            }

            _sslStream.Write(message, 0, message.Length);
        }

        private void HandleReceivedData(JObject jObject)
        {
            var command = jObject["command"].ToString();
            Console.WriteLine("Received this command: ");
            Console.WriteLine(command);
            switch (command)
            {
                case "message/send/all":
                    var messageAll = jObject["data"]["message"].ToString();
                    var senderAll = jObject["data"]["sender"].ToString();

                    Program.VrManager.SendChatMessage(messageAll, senderAll, MessageType.Global);

                    break;

                case "message/send/client":
                    var messageClient = jObject["data"]["message"].ToString();
                    var senderClient = jObject["data"]["sender"].ToString();

                    Program.VrManager.SendChatMessage(messageClient, senderClient, MessageType.Doctor);

                    break;

                case "bike/setresistance":
                    var resistance = jObject["data"]["resistance"].ToString();

                    var resistanceByte = (byte) (double.Parse(resistance) * 2);
                    
                    byte[] bytes = {0xA4, 0x09, 0x4E, 0x05, 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, resistanceByte , 0x00};
            
                    byte checkSum = 0x00;
                    for (int i = 0; i < bytes.Length - 1; i++) 
                    {
                        checkSum ^= bytes[i];
                    }
                    bytes[bytes.Length - 1] = checkSum;

                    Program.Translator.Devices[0].Write(bytes);
                    Program.VrManager.SendChatMessage( "SYSTEM", $"Resistance changed: {resistance}",
                        MessageType.System);
                    
                    // todo: maybe add to the IBLEDevice a way to identify the device

                    break;
                case "session/start":
                    Program.VrManager.StartSession();
                    Program.VrManager.SendChatMessage("Session started", "SYSTEM", MessageType.System);

                    Program.fiets.Connect();

                    // Program.BicycleEmulator.Connect();
                    Program.HeartRateMonitorEmulator.Connect();


                    Thread sendFEDataThread = new Thread(() => SendFEData());
                    sendFEDataThread.Start();

                    break;
                case "session/stop":
                    // Program.BicycleEmulator.Disconnect();
                    // Program.HeartRateMonitorEmulator.Disconnect();

                    Program.VrManager.StopSession();
                    Program.VrManager.SendChatMessage("Session stopped", "SYSTEM", MessageType.System);

                    Program.fiets.Disconnect();

                    StopSendingFEData();

                    break;

                default:
                    var status = jObject["data"]["status"].ToString();
                    if (!status.Equals("ok"))
                    {
                        var error = jObject["data"]["error"].ToString();
                        throw new Exception(error);
                    }
                    break;
            }
        }

        private JObject Read()
        {
            Console.WriteLine("reading...");
            // Create a bufer of size 4 to receive the length of the incoming message
            byte[] responseBuffer = new byte[4];
            int bytesRead = _sslStream.Read(responseBuffer, 0, responseBuffer.Length);

            // Reverse the byte array (since the received message is in big endian and convert it to an int)
            responseBuffer.Reverse();
            int responseLength = BitConverter.ToInt32(responseBuffer, 0);

            // Create a buffer to receive message from the server an read message into it until the entire message is received
            byte[] buffer = new byte[responseLength];
            int totalBytesRead = 0;

            while (totalBytesRead < responseLength)
            {
                bytesRead = _sslStream.Read(buffer, totalBytesRead, responseLength - totalBytesRead);
                totalBytesRead += bytesRead;
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

        public void SendFEData()
        {
            _runSendFEData = true;
            AllDataAttribute oldData = new AllDataAttribute();
            while (_runSendFEData)
            {
                if (Program.Data != oldData && Program.Data != null)
                {
                    if (_runSendFEData)
                    {
                        Console.WriteLine("sending new data");
                        Program.ServerConnection.Write(new DataSend() { Data = Program.Data });
                    }
                    Thread.Sleep(100);

                    oldData = Program.Data;
                }
                
            }
            
        }

        public void StopSendingFEData()
        {
            _runSendFEData = false;
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}