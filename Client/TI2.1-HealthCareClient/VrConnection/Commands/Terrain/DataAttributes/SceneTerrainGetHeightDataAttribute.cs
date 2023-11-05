using Newtonsoft.Json;

namespace TI2._1_HealthCareClient.Commands.Terrain.DataAttributes
{
    public class SceneTerrainGetHeightDataAttribute
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal[] Position { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal[][] Positions { get; set; }
    }
}
