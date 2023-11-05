using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Connections
{
    public class ConnectionConnectBikeHandler
    {
        private SslStream _stream;
        public ConnectionConnectBikeHandler(SslStream stream)
        {
            _stream = stream;
        }

        public void Handle()
        {
            ClientInfoManager.AddBike(_stream);
        }
    }
}
