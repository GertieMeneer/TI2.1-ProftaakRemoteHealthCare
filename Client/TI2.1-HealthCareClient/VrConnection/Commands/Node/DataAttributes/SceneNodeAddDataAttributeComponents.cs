using Newtonsoft.Json;

namespace TI2._1_HealthCareClient.Commands.Node.DataAttributes
{
    public class SceneNodeAddDataAttributeComponents
    {
        public SceneNodeAddDataAttributeComponentsTransform Transform { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SceneNodeAddDataAttributeComponentsModel Model { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SceneNodeAddDataAttributeComponentsTerrain Terrain { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SceneNodeAddDataAttributeComponentsPanel Panel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SceneNodeAddDataAttributeComponentsWater Water { get; set; }
    }
}
