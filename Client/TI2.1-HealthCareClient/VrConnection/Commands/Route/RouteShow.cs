using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Route.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Route
{
    public class RouteShow : IVRCommand
    {
        public string Id { get { return "route/show"; } }
        public RouteShowDataAttribute Data { get; set; }
    }
}
