using TI2._1_HealthCareClient.Commands.Route.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Route
{
    public class RouteFollow : IVRCommand
    {
        public string Id { get { return "route/follow"; } }
        public RouteFollowDataAttribute Data { get; set; }
    }
}
