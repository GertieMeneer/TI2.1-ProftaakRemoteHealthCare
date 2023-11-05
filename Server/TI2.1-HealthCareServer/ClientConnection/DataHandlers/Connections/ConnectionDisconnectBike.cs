using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Connections
{
    public class ConnectionDisconnectBike
    {
        private SslStream _stream;
        private ClientHandler _handler;
        private ClientDataManager _cdm;

        public ConnectionDisconnectBike(ClientHandler handler, SslStream stream, ClientDataManager cdm)
        {
            _stream = stream;
            _handler = handler;
            _cdm = cdm;
        }

        public void Handle()
        {
            ClientInfoManager.RemoveBike(_stream);
            _handler.Stop();
            _cdm.Stop();
            _stream.Close();
        }
    }
}
