using Newtonsoft.Json;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes
{
    public class ScenePanelDrawTextDataAttribute
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public decimal[] Position { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Size { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal[] Color { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Font { get; set; }


    }
}
