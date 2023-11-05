namespace TI2._1_HealthCareClient.VRConnection.Commands.Node.DataAttributes
{
    public class SceneNodeUpdateDataAttribute
    {
        public string id { get; set; }
        public string parent { get; set; }
        public SceneNodeUpdateTransformDataAttribute Transform { get; set; }
    }
}
