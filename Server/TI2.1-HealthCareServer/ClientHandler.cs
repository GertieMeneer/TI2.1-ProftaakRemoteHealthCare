using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.StatusMessage;
using TI2._1_HealthCareServer.ClientConnection;
using TI2._1_HealthCareServer.ClientConnection.Commands.BikeSetresistance;
using TI2._1_HealthCareServer.ClientConnection.Commands.BikeSetresistance.DataAttributes;
using TI2._1_HealthCareServer.ClientConnection.DataHandlers.Connections;
using TI2._1_HealthCareServer.ClientConnection.DataHandlers.Data;
using TI2._1_HealthCareServer.ClientConnection.DataHandlers.Messages;
using TI2._1_HealthCareServer.ClientConnection.DataHandlers.Sessions;

namespace TI2._1_HealthCareServer
{
    /// <summary>
    /// This class is responsible for handling communication with a connected client
    /// </summary>
    public class ClientHandler : ClientDataManager
    {
        private readonly SslStream _sslStream;
        private readonly TcpClient _client;

        //every client has a clienthandler, and every client has a datamanager
        private ClientDataManager _cdm = new ClientDataManager();

        private bool _run = true;

        /// <summary>
        /// Creates a clienthandler object
        /// </summary>
        /// <param name="sslStream">sslstream from client that sends data</param>
        /// <param name="client">tcpclient from client that sends data</param>
        public ClientHandler(SslStream sslStream, TcpClient client)
        {
            _sslStream = sslStream;
            _client = client;
        }


        /// <summary>
        /// Runs the client handler in a thread
        /// </summary>
        public void Start()
        {
            try
            {
                while (_run)
                {
                    var message = Communicator.Read(_sslStream, this, _cdm);
                    if (message != null)
                    {
                        Thread handleThread = new Thread(() => { HandleReceivedData(message, _client); });
                        handleThread.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _client.Close();
            }
        }

        /// <summary>
        /// Stops the client handler thread
        /// </summary>
        public void Stop()
        {
            _run = false;
        }

        /// <summary>
        /// Handles the data that was sent to the sever
        /// </summary>
        /// <param name="jObject">The data that was sent</param>
        /// <param name="tcpClient">The client who sent the data</param>
        private void HandleReceivedData(JObject jObject, TcpClient tcpClient)
        {
            var command = jObject["command"].ToString();
            switch (command)
            {
                case "connection/connect/dokter":
                    // Console.WriteLine("Dokter connect");

                    ConnectionConnectDokterHandler ccdHandler = new ConnectionConnectDokterHandler(jObject, _sslStream);
                    ccdHandler.Handle();

                    break;

                case "connection/disconnect/dokter":
                    // Console.WriteLine("Dokter disconnect");

                    ConnectionDisconnectdokterHandler cdcHandler = new ConnectionDisconnectdokterHandler();
                    cdcHandler.Handle();

                    break;

                case "connection/connect/bike":
                    // Console.WriteLine("Nieuwe fiets verbindt");

                    ConnectionConnectBikeHandler ccbHandler = new ConnectionConnectBikeHandler(_sslStream);
                    ccbHandler.Handle();

                    Communicator.WriteBack(new StatusMessage() { Command = command, Data = new StatusOk() },
                        _sslStream);

                    break;

                case "connection/disconnect/bike":
                    // Console.WriteLine("Fiets disconnect");

                    ConnectionDisconnectBike cdbHandler = new ConnectionDisconnectBike(this, _sslStream, _cdm);
                    cdbHandler.Handle();

                    break;


                case "connection/connect/patient":
                    // Console.WriteLine("Nieuwe patient logt in");

                    ConnectionConnectPatient ccpHandler = new ConnectionConnectPatient(jObject, _sslStream);
                    ccpHandler.Handle();

                    Communicator.WriteBack(new StatusMessage() { Command = command, Data = new StatusOk() },
                        _sslStream);

                    break;

                case "connection/disconnect/patient":
                    // Console.WriteLine("Patient logt uit");

                    ConnectionDisconnectPatient cdpHandler = new ConnectionDisconnectPatient(jObject, _sslStream);
                    cdpHandler.Handle();

                    _cdm.Stop();

                    Communicator.WriteBack(new StatusMessage() { Command = command, Data = new StatusOk() },
                        _sslStream);

                    Stop();

                    break;

                case "data/fetch":
                    // Console.WriteLine("Dokter vraagt data");

                    DataFetchHandler dfHandler = new DataFetchHandler(jObject);
                    dfHandler.Handle();

                    //send ok???

                    break;


                case "data/send":
                    // Console.WriteLine("Client stuurt data");

                    DataSendHandler dsHandler = new DataSendHandler(jObject, _cdm);
                    dsHandler.Handle();

                    // Communicator.WriteBack(new StatusMessage() { Command = command, Data = new StatusOk() },
                    //     _sslStream);

                    break;

                case "message/send/all":
                    // Console.WriteLine("Bericht naar iedereen");

                    MessageSendAllHandler msaHandler = new MessageSendAllHandler(jObject);
                    msaHandler.Handle();

                    Communicator.WriteToDokter(new StatusMessage() { Command = command, Data = new StatusOk() });

                    break;

                case "message/send/client":
                    // Console.WriteLine("Bericht naar specifieke client");

                    MessageSendClientHandler mscHandler = new MessageSendClientHandler(jObject);
                    mscHandler.Handle();

                    Communicator.WriteToDokter(new StatusMessage() { Command = command, Data = new StatusOk() });

                    break;

                case "session/start":
                    // Console.WriteLine("Sessie wordt gestart");

                    SessionStartHandler ssHandler = new SessionStartHandler(jObject, _cdm);
                    ssHandler.Handle();

                    Communicator.WriteToDokter(new StatusMessage() { Command = command, Data = new StatusOk() });

                    break;

                case "session/fetch/inactive":
                    // Console.WriteLine("Inactive sessions opgevraad");

                    Communicator.WriteToDokter(new StatusMessage() { Data = new StatusOk(), Command = command });

                    SessionFetchInactiveHandler sfiHandler = new SessionFetchInactiveHandler();
                    sfiHandler.Handle();

                    break;

                case "session/stop":
                    // Console.WriteLine("Sessie wordt gestopt");

                    SessionStopHandler sstopHandler = new SessionStopHandler(jObject, _cdm);
                    sstopHandler.Handle();

                    break;

                case "bike/setresistance":
                    // stuur naar _client dat bike resistance omhoog moet
                    var patientIdResistance = jObject["data"]["patientid"].ToString();
                    var resistance = int.Parse(jObject["data"]["resistance"].ToString());

                    Communicator.WriteToClient(new BikeSetResistance()
                    {
                        Data = new BikeSetResistanceData()
                            { PatientId = patientIdResistance, Resistance = resistance }
                    }, ClientInfoManager.FindBikeWithPatientId(patientIdResistance));

                    break;

                default:
                    Console.WriteLine("unknown command");
                    Communicator.WriteBack(new StatusMessage()
                        { Command = command, Data = new StatusError() { Error = "Unknown command" } }, _sslStream);
                    break;
            }
        }
    }
}