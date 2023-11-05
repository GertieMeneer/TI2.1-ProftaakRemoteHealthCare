using System;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace TI2._1_HealthCareServer
{
    /// <summary>
    /// This class is responsible for running the server and handling clients
    /// </summary>
    public class Server
    {
        private const int Port = 12345;
        private readonly TcpListener _listener;
        private X509Certificate2 serverCertificate;

        /// <summary>
        /// Create a server object. Create a listener object and load the PFX file certificate
        /// </summary>
        public Server()
        {
            _listener = new TcpListener(IPAddress.Any, Port);
            serverCertificate = new X509Certificate2("RootCA.pfx", "password");
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            _listener.Start();
            Console.WriteLine("Server started, waiting for connections...");
            
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine("A client has connected");

                //Get the netorkstream from client in sslstream format
                //False means: client.getstream gets automatically closed when sslstream is closed
                SslStream sslStream = new SslStream(client.GetStream(), false);

                try
                {
                    //Establish server's identity and handshake with client
                    sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, true);

                    //Pass connected client information to clienthandler and start the thread
                    var clientHandler = new ClientHandler(sslStream, client);
                    Thread clientThread = new Thread(() => { clientHandler.Start(); });
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}