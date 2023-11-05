using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendClient.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.MessageSendClient
{
    public class MessageSendClient : IDokterCommand
    {
        public string Command = "message/send/client";
        public MessageSendClientDataAttributes Data { get; set; }
    }
}
