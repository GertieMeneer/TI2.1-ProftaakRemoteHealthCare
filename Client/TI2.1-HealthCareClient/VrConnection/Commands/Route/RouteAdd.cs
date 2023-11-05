using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.Commands.Route.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Route
{
    public class RouteAdd : IVRCommand
    {
        public string Id { get { return "route/add"; } }
        public RouteAddDataAttribute Data { get; set; }
    }
}
