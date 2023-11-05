using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend.DataAttributes;

namespace TI2._1_HealthCareClient.ServerConnection.Commands.DataSend
{
    public class DataSend : IServerCommand
    {
        public string Command = "data/send";
        public AllDataAttribute Data { get; set; }
    }   
}