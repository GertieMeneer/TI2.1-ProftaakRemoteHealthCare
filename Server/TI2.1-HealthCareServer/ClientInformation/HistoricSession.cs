using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareServer.ClientInformation
{
    /// <summary>
    /// HistoricSession struct for storing information
    /// </summary>
    public struct HistoricSession
    {
        public string PatientName { get; set; }
        public string SessionDate { get; set; }
    }
}
