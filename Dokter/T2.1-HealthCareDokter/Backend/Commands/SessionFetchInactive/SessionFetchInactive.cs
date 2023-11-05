using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2._1_HealthCareDokter.Backend.Commands.SessionFetchInactive
{
    public class SessionFetchInactive : IDokterCommand
    {
        public string Command = "session/fetch/inactive";
    }
}
