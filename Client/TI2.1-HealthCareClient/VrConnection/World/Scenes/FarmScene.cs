using System.Drawing;
using System;
using System.IO;
using static TI2._1_HealthCareClient.VRConnection.VrClientConnection;

namespace TI2._1_HealthCareClient.VRConnection.World.Scenes
{
    /// <summary>
    /// Class that generates a Farm themed VR scene
    /// </summary>
    public class FarmScene : Scene
    {
        /// <summary>
        /// Constructor to create a farm themed VR scene
        /// </summary>
        /// <param name="name">The name of the scene</param>
        public FarmScene(string name, bool isFirstTimeSetup, bool useSimpleScene) : base(name)
        {

            if (!isFirstTimeSetup)
            {
                Load();
            }
            else
            {
                Create(useSimpleScene);
            }

            // The code below sadly does not work due to a bug in the VR server code. For this reason a simpler version is used as shown above

            //// If the scene cannot be saved, the scene already exists and should be loaded in
            //if (!Save(name, false))
            //{
            //    Load();
            //}
            //else
            //{
            //    Create();
            //}
        }

        /// <summary>
        /// Load in the VR scene
        /// </summary>
        public override void Load()
        {
            _vrClientConnection.LoadScene(Name);

            string root = FindRoot();
            Bitmap heightmap = new Bitmap(root + @"Terrain\heightmap_road.png");
            AddTerrain();

            // Create the route
            AddRoute();

            // Create the road
            _sceneBuilder.AddRoad(root + @"Road\tarmac_diffuse.png", root + @"Road\tarmac_normal.png", root + @"Road\tarmac_specular.png", 0.1m);

            // Add the vehicle
            AddVehicle();

            SetCamera();
        }

        /// <summary>
        /// Create the VR scene
        /// </summary>
        public override void Create(bool useSimpleScene)
        {

            string root = FindRoot();

            // Setup the list of models
            string[] TREES = Directory.GetFiles(root + @"Scene\Trees");
            string[] PLANTS = Directory.GetFiles(root + @"Scene\Plants");
            string[] ROCKS_AND_MISC = Directory.GetFiles(root + @"Scene\Rocks & misc");
            string[] VEGETATION = Directory.GetFiles(root + @"Scene\Vegetation");
            string[] BUSHES = Directory.GetFiles(root + @"Scene\Bushes");
            string[] BUILDINGS = Directory.GetFiles(root + @"Scene\Buildings");
            string[] ROAD_DECORATION = Directory.GetFiles(root + @"Scene\RoadDecoration");

            // Create the terrain
            AddTerrain();

            // Barn first corner
            _sceneBuilder.AddModel("Farm1", null, BUILDINGS[4], 50, 0, -30, 0.015m, 0, -20, 0);

            // Barn first and third corner
            _sceneBuilder.AddModel("Farm2", null, BUILDINGS[1], 17, 0, -70, 0.015m, 0, 60, 0);

            // Barn second corner (inside)
            _sceneBuilder.AddModel("Farm3", null, BUILDINGS[6], 12, 0, 0, 0.015m, 0, -30, 0);

            // Barn second corner (outside)
            _sceneBuilder.AddModel("Farm4", null, BUILDINGS[3], 40, 0, 30, 0.015m, 0, 35, 0);

            // Silo third corner
            _sceneBuilder.AddModel("Silo1", null, BUILDINGS[5], -20, 0, -60, 0.015m, 0, -30, 0);

            // Barn somewhat fourth corner
            _sceneBuilder.AddModel("Farm5", null, BUILDINGS[1], -25, 0, 20, 0.015m, 0, 0, 0);

            // Silo 5th corner
            _sceneBuilder.AddModel("Silo2", null, BUILDINGS[2], -75, 0, 27, 0.015m, 0, 45, 0);

            // Silo final corner
            _sceneBuilder.AddModel("Silo3", null, BUILDINGS[5], 47, 0, 53, 0.015m, 0, 35, 0);

            //Streetlights
            Bitmap weightMap = new Bitmap(root + @"Terrain\streetlights_north.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, 180);

            weightMap = new Bitmap(root + @"Terrain\streetlights_north_east.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, 135);

            weightMap = new Bitmap(root + @"Terrain\streetlights_east.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, 90);

            weightMap = new Bitmap(root + @"Terrain\streetlights_south_east.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, 45);

            weightMap = new Bitmap(root + @"Terrain\streetlights_south.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, 0);

            weightMap = new Bitmap(root + @"Terrain\streetlights_south_west.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, -45);

            weightMap = new Bitmap(root + @"Terrain\streetlights_west.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, -90);

            weightMap = new Bitmap(root + @"Terrain\streetlights_north_west.png");
            _sceneBuilder.scatterModel("StreetLantern", null, ROAD_DECORATION[1], weightMap, -0.25m, 0.01m, 0, false, 0, -135);

            if (!useSimpleScene)
            {
                // Add trees, bushes and plants

                //Bitmap weightmap = new Bitmap(root + @"Terrain\Weightmap - test.png");
                //_sceneBuilder.scatterModel("Tree", null, TREES, weightmap, 0.01m, 0.01m, 0.003m, true);

                //weightmap = new Bitmap(root + @"Terrain\Weightmap_plants.png");
                //_sceneBuilder.scatterModel("Plant", null, PLANTS, weightmap, 0.01m, 0.01m, 0.003m, true);

                //weightmap = new Bitmap(root + @"Terrain\weightmap_rocks.png");
                //_sceneBuilder.scatterModel("Tree", null, ROCKS_AND_MISC, weightmap, 0.01m, 0.01m, 0.003m, true);

                //weightmap = new Bitmap(root + @"Terrain\weightmap_bushes.png");
                //_sceneBuilder.scatterModel("Tree", null, BUSHES, weightmap, 0.01m, 0.01m, 0.003m, true);

                //weightmap = new Bitmap(root + @"Terrain\weightmap_vegetation.png");
                //_sceneBuilder.scatterModel("Tree", null, VEGETATION, weightmap, 0.01m, 0.01m, 0.003m, true);
            }

            // Save the scene
            _sceneBuilder.save(Name, true);


            // Create the route
            AddRoute();

            // Create the road
            _sceneBuilder.AddRoad(root + @"Road\tarmac_diffuse.png", root + @"Road\tarmac_normal.png", root + @"Road\tarmac_specular.png", 0.1m);

            // Add the vehicle
            AddVehicle();

            SetCamera();

        }

        /// <summary>
        /// Find the root directory
        /// </summary>
        /// <returns>The absolute filepath of the route directory</returns>
        private string FindRoot()
        {
            string dir = Directory.GetCurrentDirectory();
            return dir.Substring(0, dir.Length - 9) + @"Resources\";
        }

        /// <summary>
        /// Add the terrain to the scene
        /// </summary>
        private void AddTerrain()
        {
            string root = FindRoot();

            Bitmap heightmap = new Bitmap(root + @"Terrain\heightmap_road.png");
            _sceneBuilder.AddTerrain(root + @"Terrain\Grass.jpg", root + @"Terrain\grass_normal.png", heightmap, 10);
        }

        /// <summary>
        /// Add the route to the scene
        /// </summary>
        private void AddRoute()
        {
            // First straight
            _sceneBuilder.AddRouteSegment(55, 0, 55, 0, 0, -30);
            _sceneBuilder.AddRouteSegment(0, 0, -40, 0, 0, -15);

            // First Chicane
            _sceneBuilder.AddRouteSegment(15, 0, -15, 0, 0, -15);

            // Second straight
            _sceneBuilder.AddRouteSegment(0, 0, -40, 0, 0, -85);

            // First turn
            _sceneBuilder.AddRouteSegment(-40, 0, 0, 0, 0, 85);

            // Third straight
            _sceneBuilder.AddRouteSegment(0, 0, 50, 0, 0, 60);

            // second turn
            _sceneBuilder.AddRouteSegment(-35, 0, 0, 0, 0, -60);

            // Fourth straight
            _sceneBuilder.AddRouteSegment(0, 0, -75, 0, 0, -45);

            // Third turn
            _sceneBuilder.AddRouteSegment(-25, 0, 0, 0, 0, 45);

            // Second chicane
            _sceneBuilder.AddRouteSegment(0, 0, 30, 0, 0, 15);
            _sceneBuilder.AddRouteSegment(-10, 0, 10, 0, 0, 15);

            // Fifth straight
            _sceneBuilder.AddRouteSegment(0, 0, 20, 0, 0, 60);

            // Fourth corner
            _sceneBuilder.AddRouteSegment(-20, 0, 20, 0, 0, 0);

            // Sixth straight
            _sceneBuilder.AddRouteSegment(-20, 0, 0, -45, 0, 0);

            // Fith corner
            _sceneBuilder.AddRouteSegment(0, 0, 25, 45, 0, 0);

            // Final straight and corner
            _sceneBuilder.AddRouteSegment(105, 0, 0, 30, 0, 0);

            _sceneBuilder.AddRouteSegment(15, 0, 15, 0, 0, 30);

            // Finalize the route
            _sceneBuilder.AddRoute();
        }

        private void AddVehicle()
        {
            string root = FindRoot();
            _sceneBuilder.AddVehicle(root + @"Vehicle\bike.fbx", true, root + @"Vehicle\bike_anim.fbx");
            _sceneBuilder.followRoute(0, 0, RouteRotation.XYZ, 1.0m, true, 0m, -(decimal)(Math.PI * 0.5), 0);
        }

        private void SetCamera()
        {
            _sceneBuilder.SetCamera();
        }
    }
}
