using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.General.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.General
{
    public class Pause : IVRCommand
    {
        public string Id { get { return "pause"; } }
        public PauseDataAttribute Data { get { return new PauseDataAttribute(); } }
    }
}
