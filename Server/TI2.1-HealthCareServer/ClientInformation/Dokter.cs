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
    /// Class responsible for creating dokter object
    /// </summary>
    public static class Dokter
    {
        private static SslStream _dokter = null;

        /// <summary>
        /// Get the dokter tcpclient object
        /// </summary>
        /// <returns>TcpClient object from the dokter</returns>
        public static SslStream GetDokter()
        {
            return _dokter;
        }

        /// <summary>
        /// Sets a dokter
        /// </summary>
        /// <param name="dokterTcp">TcpClient object for the dokter</param>
        /// <returns>True if can add, false if there's already a dokter set aka connected</returns>
        public static bool AddDokter(SslStream sslStream)
        {
            if (_dokter == null)
            {
                _dokter = sslStream;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove the dokter
        /// </summary>
        /// <param name="dokterTcp">TcpClient object from the dokter</param>
        public static void RemoveDokter()
        {
            _dokter = null;
        }

    }
}
