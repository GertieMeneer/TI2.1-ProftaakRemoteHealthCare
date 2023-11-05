using Newtonsoft.Json.Linq;

namespace TI2._1_HealthCareClient.VRConnection.World.Scenes
{
    /// <summary>
    /// Abstract class to create a VR scene
    /// </summary>
    public abstract class Scene
    {
        protected VrClientConnection _vrClientConnection = VrClientConnection.GetInstance();
        protected SceneBuilder _sceneBuilder = new SceneBuilder();

        public string Name { get; private set; }

        /// <summary>
        /// Constructor th create a VR scene. Also resets the VR scene
        /// </summary>
        /// <param name="name">The name of the scene</param>
        public Scene(string name)
        {
            Name = name;

            // reset the scene and remove the ground plane
            _vrClientConnection.ResetScene();
            _vrClientConnection.DeleteNode(_vrClientConnection.FindUUIDByName("GroundPlane"));
        }

        /// <summary>
        /// Creata a VR scene
        /// </summary>
        public abstract void Create(bool useSimpleScene);

        /// <summary>
        /// Save a VR scene
        /// </summary>
        public bool Save(string name, bool overwrite)
        {
            JObject jObject = _vrClientConnection.SaveScene(name, overwrite);

            return jObject["data"]["data"]["status"].ToString() == "ok";
        }

        /// <summary>
        /// Load a VR scene
        /// </summary>
        public abstract void Load();
    }
}
