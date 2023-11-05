using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.SessionStart.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.SessionStart
{
    public class SessionStart : IDokterCommand
    {
        public string Command = "session/start";
        public SessionStartDataAttributes Data { get; set; }
    }
}
