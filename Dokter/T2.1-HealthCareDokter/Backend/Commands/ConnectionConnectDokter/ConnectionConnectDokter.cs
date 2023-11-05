using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.ConnectionConnectDokter.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.ConnectionConnectDokter
{
    public class ConnectionConnectDokter : IDokterCommand
    {
        public string Command = "connection/connect/dokter";
        public ConnectionConnectDokterDataAttribute Data { get; set; }
    }
}
