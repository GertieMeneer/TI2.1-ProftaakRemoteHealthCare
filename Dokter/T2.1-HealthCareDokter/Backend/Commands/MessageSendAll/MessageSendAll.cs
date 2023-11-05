using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendAll.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.MessageSendAll
{
    public class MessageSendAll : IDokterCommand
    {
        public string Command = "message/send/all";
        public MessageSendAllDataAttribute Data { get; set; }
    }
}
