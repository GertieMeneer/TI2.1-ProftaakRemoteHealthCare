namespace TI2._1_HealthCareClient.Commands.Route.DataAttributes
{
    public class RouteFollowDataAttribute
    {
        public string Route { get; set; }
        public string Node { get; set; }
        public decimal Speed { get; set; }
        public decimal Offset { get; set; }
        public string Rotate { get; set; }
        public decimal Smoothing { get; set; }
        public bool FollowHeight { get; set; }
        public decimal[] RotateOffset { get; set; }
        public decimal[] positionOffset { get; set; }
    }
}
