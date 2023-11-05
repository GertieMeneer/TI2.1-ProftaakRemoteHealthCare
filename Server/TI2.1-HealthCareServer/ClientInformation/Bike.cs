using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareServer.ClientInformation
{
    /// <summary>
    /// Class responsible for storing information about bike
    /// </summary>
    public class Bike
    {
        public SslStream Stream { get; set; }
        public bool HasPatient { get; set; }
        public string PatientId { get; set; }

        /// <summary>
        /// Constructor for setting up a bike object
        /// </summary>
        /// <param name="stream">SslStream that belongs to the right client, aka bike</param>
        public Bike(SslStream stream)
        {
            Stream = stream;
            HasPatient = false;
            PatientId = null;
        }
    }
}