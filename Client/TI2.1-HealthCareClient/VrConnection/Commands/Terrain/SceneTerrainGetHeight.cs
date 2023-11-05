using TI2._1_HealthCareClient.Commands.Terrain.DataAttributes;

namespace TI2._1_HealthCareClient.Commands.Terrain
{
    public class SceneTerrainGetHeight : IVRCommand
    {
        public string Id { get { return "scene/terrain/getheight"; } }
        public SceneTerrainGetHeightDataAttribute Data { get; set; }
    }
}
