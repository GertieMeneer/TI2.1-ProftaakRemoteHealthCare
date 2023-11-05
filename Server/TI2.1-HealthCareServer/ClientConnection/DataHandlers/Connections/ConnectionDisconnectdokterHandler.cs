using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareServer.ClientInformation;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Connections
{
    public class ConnectionDisconnectdokterHandler
    {
        public void Handle()
        {
            Dokter.RemoveDokter();
        }
    }
}
