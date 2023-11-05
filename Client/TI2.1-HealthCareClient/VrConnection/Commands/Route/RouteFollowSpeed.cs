using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.VRConnection.Commands.Route.DataAttributes;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Route
{
    public class RouteFollowSpeed : IVRCommand
    {
        public string Id { get { return "route/follow/speed"; } }
        public RouteFollowSpeedDataAttribute Data { get; set; }
    }
}
