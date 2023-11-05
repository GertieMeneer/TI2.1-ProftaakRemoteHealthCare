using TI2._1_HealthCareClient.Commands.Terrain.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Terrain
{
    public class SceneTerrainAdd : IVRCommand
    {
        public string Id { get { return "scene/terrain/add"; } }
        public SceneTerrainAddDataAttribute Data { get; set; }
    }
}
