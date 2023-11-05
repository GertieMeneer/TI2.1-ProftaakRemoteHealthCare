using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TI2._1_HealthCareClient.Commands.Route.DataAttributes;
using static TI2._1_HealthCareClient.VRConnection.VrClientConnection;

namespace TI2._1_HealthCareClient.VRConnection.World
{
    /// <summary>
    /// Class responsible for creating easy methods for managing a scene in VR
    /// </summary>
    public class SceneBuilder
    {
        private VrClientConnection _vrClientConnection = VrClientConnection.GetInstance() ;
        
        private decimal[][] _heights;

        private List<RouteAddDataAttributeNode> _route = new List<RouteAddDataAttributeNode>();
        private string _routeUUID;

        /// <summary>
        /// Saves a scene in a specified file
        /// </summary>
        /// <param _panelUUID="fileName">The _panelUUID of the file to which the scene should be written</param>
        /// <param _panelUUID="overwrite">true = the excisting file should be overwritten, false = the excisting file should not be overwritten</param>
        public void save(string filename, bool overwrite)
        {
            _vrClientConnection.SaveScene(filename, overwrite);
        }

        /// <summary>
        /// Loads a scene from a specified file
        /// </summary>
        /// <param _panelUUID="fileName">The _panelUUID of the scene file</param>
        public void load(string filename)
        {
            _vrClientConnection.LoadScene(filename);
        }

        /// <summary>
        /// Generate a heightmap based on a heightmap image and the maximumheight
        /// </summary>
        /// <param _panelUUID="heightMap">Heigtmap image</param
        /// <param _panelUUID="maxHeight">The maximum height of the heightmap</param>
        /// <returns>A heightmap with values between 0 and the maxheight</returns>
        private decimal[] generateHeightMap(Bitmap heightMap, int maxHeight)
        {
            // get the width and height of the heightMap
            int width = heightMap.Width;
            int height = heightMap.Height;

            // Create a new array to store the smoothed heightmap
            decimal[] smoothedHeightMap = new decimal[width * height];

            // Create a new array to store the heights
            _heights = new decimal[width][];

            // Loop through the heightmap pixels
            for (int x = 0; x < heightMap.Width; x++)
            {
                _heights[x] = new decimal[height];

                for (int z = 0; z < height; z++)
                {
                    int currentIndex = z * width + x;

                    // Calculate the average value of the pixel and its neighboring pixels
                    decimal sum = (decimal)heightMap.GetPixel(x, z).GetBrightness();
                    int count = 1;

                    // Check neighboring pixels
                    if (x > 0)
                    {
                        sum += (decimal)heightMap.GetPixel(x - 1, z).GetBrightness();
                        count++;
                    }
                    if (x < width - 1)
                    {
                        sum += (decimal)heightMap.GetPixel(x + 1, z).GetBrightness();
                        count++;
                    }
                    if (z > 0)
                    {
                        sum += (decimal)heightMap.GetPixel(x, z - 1).GetBrightness();
                        count++;
                    }
                    if (z < height - 1)
                    {
                        sum += (decimal)heightMap.GetPixel(x, z + 1).GetBrightness();
                        count++;
                    }

                    // Calculate the average and scale it to the desired maxHeight
                    decimal average = sum / count;
                    smoothedHeightMap[currentIndex] = average * maxHeight;
                    _heights[x][z] = average * maxHeight;
                }
            }
            return smoothedHeightMap;
        }

        /// <summary>
        /// Add a terrain based on a given heightmap
        /// </summary>
        /// <param _panelUUID="diffusePath">The ground texture of the terrain</param>
        /// <param _panelUUID="normalPath">The normal map of the ground texture</param>
        /// <param _panelUUID="heightMap">The heightmap to be used by the terrain</param>
        /// <param _panelUUID="maxHeight">The height that equals to the color white on the heightmap</param>
        public void AddTerrain(string diffusePath, string normalPath, Bitmap heightMap, int maxHeight)
        {
            decimal[] heights = generateHeightMap(heightMap, maxHeight);
            _vrClientConnection.AddTerrain(heightMap.Width, heightMap.Height, heights);
            _vrClientConnection.AddTerrainNode("Terrain", null, -heightMap.Width / 2, 0, -heightMap.Height / 2);
            _vrClientConnection.AddLayer("Terrain", diffusePath, normalPath, 0, maxHeight, 5);
        }

        /// <summary>
        /// Generate a weightmap based on a weighttmap image
        /// </summary>
        /// <param _panelUUID="weigthMap">Weightmap image</param>
        /// <returns></returns>
        private decimal[][] generateWeightMap(Bitmap weigthMap)
        {
            // get the width and height of the weightMap
            int width = weigthMap.Width;
            int height = weigthMap.Height;


            // Create a new array to store the weights
            decimal[][] weights = new decimal[width][];

            for (int x = 0; x < width; x++)
            {
                weights[x] = new decimal[height];

                for (int z = 0; z < height; z++)
                {
                    weights[x][z] = (decimal)weigthMap.GetPixel(x, z).GetBrightness(); ;
                }
            }
            return weights;
        }

        /// <summary>
        /// Scatters a model randomly as defined by a weightmap
        /// </summary>
        /// <param _panelUUID="name">The _panelUUID inside the VR server (will get a number as suffix)</param>
        /// <param _panelUUID="parentName">The _panelUUID of parent in VR</param>
        /// <param _panelUUID="modelFilePath">The filepath of the model</param>
        /// <param _panelUUID="weightMap">The weightmap</param>
        /// <param _panelUUID="zPositionOffset">Offset with the z axis to prevent floating models</param>
        /// <param _panelUUID="scale">The scale of the model</param>
        /// <param _panelUUID="scaleOffset">The allowed variation in scale (set to 0 for no variation)</param>
        /// <param _panelUUID="yRotationRandom">false = the y rotation will not be randomized, true = the y rotation will be randomized</param>
        /// <param _panelUUID="xRotationOffset">The x rotation of the model</param>
        /// <param _panelUUID="yRotationOffset">The y rotation of the model</param>
        /// <param _panelUUID="zRotationOffset">The z rotation of the model</param>
        public void scatterModel(string name, string parentName, string modelFilePath, Bitmap weightMap, decimal zPositionOffset = 0.1m, decimal scale = 1, decimal scaleOffset = 0, bool yRotationRandom = true, decimal xRotationOffset = 0, decimal yRotationOffset = 0, decimal zRotationOffset = 0)
        {
            scatterModel(name, parentName, new string[] { modelFilePath }, weightMap, zPositionOffset, scale, scaleOffset, yRotationRandom, xRotationOffset, yRotationOffset, zRotationOffset);
        }

        /// <summary>
        /// Scatters a set of models randomly as defined by a weightmap. The weights of the individual model are equal. 
        /// </summary>
        /// <param _panelUUID="name">The _panelUUID inside the VR server (will get a number as suffix)</param>
        /// <param _panelUUID="parentName">The _panelUUID of parent in VR</param>
        /// <param _panelUUID="modelFilePaths">The filepaths of the models</param>
        /// <param _panelUUID="weightMap">The weightmap</param>
        /// <param _panelUUID="zPositionOffset">Offset with the z axis to prevent floating models</param>
        /// <param _panelUUID="scale">The scale of the model</param>
        /// <param _panelUUID="scaleOffset">The allowed variation in scale (set to 0 for no variation)</param>
        /// <param _panelUUID="yRotationRandom">false = the y rotation will not be randomized, true = the y rotation will be randomized</param>
        /// <param _panelUUID="xRotationOffset">The x rotation of the model</param>
        /// <param _panelUUID="yRotationOffset">The y rotation of the model</param>
        /// <param _panelUUID="zRotationOffset">The z rotation of the model</param>
        public void scatterModel(string name, string parentName, string[] modelFilePaths, Bitmap weightMap, decimal zPositionOffset = 0.1m, decimal scale = 1, decimal scaleOffset = 0, bool yRotationRandom = true, decimal xRotationOffset = 0, decimal yRotationOffset = 0, decimal zRotationOffset = 0)
        {
            Random random = new Random();

            decimal[][] probabilities = generateWeightMap(weightMap);
            
            // get the width and height of the weightMap
            int width = weightMap.Width;
            int height = weightMap.Height;

            decimal randomScale = scale;
            decimal RandomRotation = yRotationOffset;

            int successCount = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if ((decimal)random.NextDouble() < probabilities[x][y])
                    {
                        if (scaleOffset > 0) randomScale = (decimal)(random.NextDouble()) * (2 * scaleOffset) + (scale - scaleOffset);
                        if (yRotationRandom) RandomRotation = new decimal(random.NextDouble() * 360);

                        _vrClientConnection.AddModelNode(name + successCount, _vrClientConnection.FindUUIDByName(parentName), modelFilePaths[random.Next(0, modelFilePaths.Length)], -width/2 + x, _heights[x][y] + zPositionOffset, -width/2 + y , randomScale, xRotationOffset, RandomRotation, zRotationOffset);
                        successCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a mode to the VR Scene
        /// </summary>
        /// <param _panelUUID="name">Name for the model</param>
        /// <param _panelUUID="parent">Parent _panelUUID</param>
        /// <param _panelUUID="filepath">filepath of the model</param>
        /// <param _panelUUID="xPostion">x position of the model</param>
        /// <param _panelUUID="yPostion">y position of the model</param>
        /// <param _panelUUID="zPosition">z position of the model</param>
        /// <param _panelUUID="scale">scale of the model</param>
        /// <param _panelUUID="xRotation">x rotation of the model</param>
        /// <param _panelUUID="yRotation">y rotation of the model</param>
        /// <param _panelUUID="zRotation">z rotation of the model</param>
        public void AddModel(string name, string parent, string filepath, int xPostion, int yOffset, int zPostion, decimal scale, decimal xRotation, decimal yRotation, decimal zRotation)
        {
            _vrClientConnection.AddModelNode(name, parent, filepath, xPostion, _heights[_heights.Length/2 + xPostion][_heights[0].Length/2 + zPostion] + yOffset, zPostion , scale, xRotation, yRotation, zRotation);
        }

        /// <summary>
        /// Add a new route segment relative to the last route segment. If no previous segment was placed, the coordinates are absolute
        /// </summary>
        /// <param _panelUUID="xDifference">The x displacement compared to the previous route segment</param>
        /// <param _panelUUID="yDifference">The y displacement compared to the previous route segment</param>
        /// <param _panelUUID="zDifference">The z displacement compared to the previous route segment</param>
        /// <param _panelUUID="xVector">The x value of the vector of the route segment</param>
        /// <param _panelUUID="yVector">The y value of the vector of the route segment</param>
        /// <param _panelUUID="zVector">The z value of the vector of the route segment</param>
        public void AddRouteSegment(decimal xDifference, decimal yDifference, decimal zDifference, decimal xVector, decimal yVector, decimal zVector)
        {
            RouteAddDataAttributeNode lastNode = _route.LastOrDefault<RouteAddDataAttributeNode>();
            if (lastNode == null)
            {
                _route.Add(new RouteAddDataAttributeNode() { Pos = new decimal[] {xDifference, yDifference, zDifference, }, Dir = new decimal[] { xVector, yVector, zVector } });
            }
            else
            {
                _route.Add(new RouteAddDataAttributeNode() { Pos = new decimal[] { lastNode.Pos[0] + xDifference, lastNode.Pos[1] + yDifference, lastNode.Pos[2] + zDifference, }, Dir = new decimal[] { xVector, yVector, zVector } });
            }
        }

        /// <summary>
        /// Add a new route segment
        /// </summary>
        /// <param _panelUUID="xDifference">The x location of the route segment</param>
        /// <param _panelUUID="yDifference">The y location of the route segment</param>
        /// <param _panelUUID="zDifference">The z location of the route segment</param>
        /// <param _panelUUID="xVector">The x value of the vector of the route segment</param>
        /// <param _panelUUID="yVector">The y value of the vector of the route segment</param>
        /// <param _panelUUID="zVector">The z value of the vector of the route segment</param>
        public void AddNode(int x, int y, int z, int xVector, int yVector, int zVector)
        {
            _route.Add(new RouteAddDataAttributeNode() { Pos = new decimal[] { x, y, z }, Dir = new decimal[] { xVector, yVector, zVector } });
        }

        /// <summary>
        /// Add the created route to the VR scene
        /// </summary>
        public void AddRoute()
        {
            if (_route == null)
            { throw new Exception("No route was created");
            }
            else
            {
                _routeUUID = _vrClientConnection.AddRoute(_route.ToArray());
            }
        }

        /// <summary>
        /// Adds a visable road to the VR scene
        /// </summary>
        /// <param _panelUUID="diffusePath">The filepath of the diffuse texture</param>
        /// <param _panelUUID="normal">The filepath of the normalmap</param>
        /// <param _panelUUID="specular">The filepath of the specularmap</param>
        /// <param _panelUUID="heightOffset">The offset of the roadtexture compared to the route (to avoid clipping)</param>
        public void AddRoad(string diffusePath, string normal, string specular, decimal heightOffset)
        {
            _vrClientConnection.AddRoad(_routeUUID, diffusePath, normal, specular, heightOffset);
        }

        /// <summary>
        /// Adds a vechicle to the scene
        /// </summary>
        /// <param name="filePath">The filepath to the model of the vehicle</param>
        /// <param name="animated">false = the model is not animated, true = the model is animated</param>
        /// <param name="animationName">The filepath to the animation file of the vehicle</param>
        public void AddVehicle(string filePath, bool animated, string animationName)
        {
            _vrClientConnection.AddModelNode("Vehicle",null, filePath, 0, 0, 0, 1, 0, 0, 0, true, animated, animationName);
        }

        public void followRoute(decimal speed, decimal offset, RouteRotation routeRotation, decimal smoothing, bool followHeight, decimal xRotationOffset, decimal yRotationOffset, decimal zRottationOffset)
        {
            _vrClientConnection.FollowRoute(_routeUUID, _vrClientConnection.FindUUIDByName("Vehicle"), speed, offset, routeRotation, smoothing, followHeight, new decimal[] { xRotationOffset, yRotationOffset, zRottationOffset }, new decimal[3]);
        }

        /// <summary>
        /// Sets the camera on the bike
        /// </summary>
        public void SetCamera()
        {
            _vrClientConnection.UpdateNode(_vrClientConnection.FindUUIDByName("Camera"),
                _vrClientConnection.FindUUIDByName("Vehicle"));
        }
    }
}
