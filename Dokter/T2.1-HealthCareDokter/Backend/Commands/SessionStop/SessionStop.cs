using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.SessionStop.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.SessionStop
{
    public class SessionStop : IDokterCommand
    {
        public string Command = "session/stop";
        public SessionStopDataAttributes Data { get; set; }
    }
}
