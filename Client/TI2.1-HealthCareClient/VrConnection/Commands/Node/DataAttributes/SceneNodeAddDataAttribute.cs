using Newtonsoft.Json;

namespace TI2._1_HealthCareClient.Commands.Node.DataAttributes
{
    public class SceneNodeAddDataAttribute
    {
        public string Name { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }
        public SceneNodeAddDataAttributeComponents Components { get; set; }
    }
}
