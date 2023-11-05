using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TI2._1_HealthCareClient.Commands;
using TI2._1_HealthCareClient.Commands.General.DataAttributes;
using TI2._1_HealthCareClient.Commands.Node.DataAttributes;
using TI2._1_HealthCareClient.Commands.Node;
using TI2._1_HealthCareClient.Commands.Scene;
using TI2._1_HealthCareClient.Commands.Scene.DataAttributes;
using TI2._1_HealthCareClient.Commands.Terrain.DataAttributes;
using TI2._1_HealthCareClient.Commands.Terrain;
using TI2._1_HealthCareClient.Commands.SkyBox;
using TI2._1_HealthCareClient.Commands.SkyBox.DataAttributes;
using TI2._1_HealthCareClient.Commands.Road.DataAttributes;
using TI2._1_HealthCareClient.Commands.Road;
using TI2._1_HealthCareClient.Commands.Route.DataAttributes;
using TI2._1_HealthCareClient.Commands.Route;
using TI2._1_HealthCareClient.VrConnection.Commands.General;
using TI2._1_HealthCareClient.VrConnectionCommands.General.DataAttributes;
using TI2._1_HealthCareClient.VRConnection.Commands.Panel;
using TI2._1_HealthCareClient.VRConnection.Commands.Panel.DataAttributes;
using TI2._1_HealthCareClient.VRConnection.Commands.Route;
using TI2._1_HealthCareClient.VRConnection.Commands.Route.DataAttributes;
using TI2._1_HealthCareClient.VRConnection.Commands.General;
using TI2._1_HealthCareClient.VRConnection.Commands.Node;
using TI2._1_HealthCareClient.VRConnection.Commands.Node.DataAttributes;

/// <summary>
/// Class that is responsible for managing a connection with the VR Server. Implements the singleton design pattern
/// </summary>
namespace TI2._1_HealthCareClient.VRConnection
{
    public class VrClientConnection
    {
        private static VrClientConnection _instance = null;
        private static readonly object _lockObject = new object();

        private static TcpClient _client;
        private static NetworkStream _stream;
        private static string _session;

        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// Private constructor of the VrClientConnection
        /// </summary>
        private VrClientConnection()
        {
        }

        /// <summary>
        /// Get the single instance of the VrClientConnection. If no instance is created, one is created.
        /// </summary>
        public static VrClientConnection GetInstance()
        {
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new VrClientConnection();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Connect with the server and create a tunnel through which commands can be send
        /// </summary>
        /// <param _panelUUID="host">The IP adress of the server</param>
        /// <param _panelUUID="port">The port of the connection</param>
        /// <param _panelUUID="deviceName">The device _panelUUID that runs the VR simulator</param>
        public void Connect(string ipAddress, int port, string deviceName)
        {
            // Try to connect to the server using a TCP client
            _client = new TcpClient(ipAddress, port);

            // Check if the connection has suceeded
            if (!_client.Connected)
            {
                throw new Exception($"Failed to connected with Server. IP: {ipAddress} on port {port}");
            }

            _stream = _client.GetStream();
            Console.WriteLine($"Connected with Server. IP: {ipAddress} on port {port}");

            // Get a list of the connected devices and try to find "deviceName"
            JObject jsonObject = Write(new SessionList());
            JArray dataArray = (JArray)jsonObject["data"];

            JToken targetObject = dataArray.FirstOrDefault(item =>
            {
                var clientInfo = item["clientinfo"];
                return clientInfo != null && (string)clientInfo["host"] == deviceName;
            });

            if (targetObject == null)
            {
                throw new Exception($"No session on '{deviceName}' found");
            }

            Console.WriteLine($"Session on '{deviceName}' found");

            // Create a tunnel using the device id and extract resonse
            String id = (string)targetObject["id"];

            TunnelCreateDataAttribute tunnelCreateDataAttribute = new TunnelCreateDataAttribute()
                { Key = "", Session = id };
            TunnelCreate tunnelCreate = new TunnelCreate() { Data = tunnelCreateDataAttribute };
            jsonObject = Write(tunnelCreate);

            // Get the sesion ID to be used to send commands to the server
            _session = (string)jsonObject["data"]["id"];
            Console.WriteLine("Tunnel created");
            Console.WriteLine();
        }

        /// <summary>
        /// Disconnect from the VR server
        /// </summary>
        public void Disconnect()
        {
            // If no connection was established, do nothing
            if (!_client.Connected)
            {
                Console.WriteLine("Already disconnected from Server.");
                return;
            }

            // Else, close the connection
            _stream.Close();
            _client.Close();
            Console.WriteLine("Disconnected from Server");
        }

        /// <summary>
        /// Write a command to the VR server
        /// </summary>
        /// <param _panelUUID="command">Command that should be send to the server</param>
        /// <returns>Response from the server</returns>
        private JObject Write(IVRCommand ivrCommand)
        {
            // Set the serializer settings. JSON formatting should be maintained, while the entire structure should be in lowercase
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            // Format the command to a JSON array using the settings above
            byte[] jsonArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(ivrCommand, serializerSettings));

            // Get the length of the JSON message and append it to the beginning of message
            byte[] length = BitConverter.GetBytes((uint)jsonArray.Length);
            byte[] message = length.Concat(jsonArray).ToArray();

            // Send the message to the server

            if (DebugMode)
            {
                Console.WriteLine("Sending message:");
                Console.WriteLine(JsonConvert.SerializeObject(ivrCommand, serializerSettings));
                Console.WriteLine("\n");
            }
            else
            {
                JObject jsonObject = JObject.Parse(JsonConvert.SerializeObject(ivrCommand, serializerSettings));

                if ((jsonObject["id"].ToString() != "session/list") && (jsonObject["id"].ToString() != "tunnel/create"))
                {
                    Console.WriteLine("Sending command to VR server: " + jsonObject["data"]["data"]["id"]);
                }
            }

            _stream.Write(message, 0, message.Length);

            // Get the response of the server
            return Read();
        }

        /// <summary>
        /// Read incoming message from the server
        /// </summary>
        /// <returns>
        /// Transform read from the message converted to JSON
        /// </returns>
        private JObject Read()
        {
            // Create a bufer of size 4 to receive the length of the incoming message
            byte[] responseBuffer = new byte[4];
            int bytesRead = _stream.Read(responseBuffer, 0, responseBuffer.Length);

            // Reverse the byte array (since the received message is in big endian and convert it to an int)
            responseBuffer.Reverse();
            int responseLength = BitConverter.ToInt32(responseBuffer, 0);

            // Create a buffer to receive message from the server an read message into it until the entire message is received
            byte[] buffer = new byte[responseLength];
            int totalBytesRead = 0;

            while (totalBytesRead < responseLength)
            {
                bytesRead = _stream.Read(buffer, totalBytesRead, responseLength - totalBytesRead);
                totalBytesRead += bytesRead;
            }

            // Set the serializer settings. JSON formatting should be maintained, while the entire structure should be in lowercase
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            // Deserialize using the settings created above
            JObject message =
                JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(buffer, 0, responseLength),
                    serializerSettings);
            if (DebugMode)
            {
                Console.WriteLine("Transform received:");
                Console.WriteLine(message);
                Console.WriteLine("\n");
            }

            return message;
        }

        /// <summary>
        /// Get the current VR scene
        /// </summary>
        /// <returns>The current VR sceneGet in JSON format</returns>
        public JObject GetScene()
        {
            SceneGet sceneGet = new SceneGet();

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneGet };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Reset the current VR scene to the default scene
        /// </summary>
        public JObject ResetScene()
        {
            SceneReset sceneReset = new SceneReset();

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneReset };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Saves a scene in a specified file
        /// </summary>
        /// <param _panelUUID="fileName">The _panelUUID of the file to which the scene should be written</param>
        /// <param _panelUUID="overwrite">true = the excisting file should be overwritten, false = the excisting file should not be overwritten</param>
        public JObject SaveScene(string fileName, bool overwrite = true)
        {
            SceneSaveDataAttribute data = new SceneSaveDataAttribute() { FileName = fileName, Overwrite = overwrite };
            SceneSave sceneSave = new SceneSave() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneSave };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Loads a scene from a specified file
        /// </summary>
        /// <param _panelUUID="fileName">The _panelUUID of the scene file</param>
        public JObject LoadScene(string fileName)
        {
            SceneLoadDataAttribute data = new SceneLoadDataAttribute() { FileName = fileName };
            SceneLoad sceneLoad = new SceneLoad() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneLoad };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            JObject jObject = Write(tunnelSend);
            DeleteNode(FindUUIDByName("Terrain")); // TODO not super clean, but will work

            return jObject;
        }

        /// <summary>
        /// Adds a model to the scene
        /// </summary>
        /// <param _panelUUID="panelUUID">The _panelUUID of the model</param>
        /// <param _panelUUID="filePath">The filepath of the model</param>
        /// <param _panelUUID="cullBackFaces">false = backfaces are not drawn, true = backfaces are drawn</param>
        /// <param _panelUUID="animated">false = model is animated, true = model is not animated</param>
        /// <param _panelUUID="animationPath">The filepath of the animation file</param>
        /// <param _panelUUID="parentUUID">The parent of the model</param>
        /// <param _panelUUID="xPosition">x position in the scene</param>
        /// <param _panelUUID="yPosition">y position in the scene</param>
        /// <param _panelUUID="zPosition">z position in the scene</param>
        /// <param _panelUUID="scale">Scaling factor in the scene</param>
        /// <param _panelUUID="xRotation">x rotation in the scene</param>
        /// <param _panelUUID="yRotation">y rotation in the scene</param>
        /// <param _panelUUID="zRotation">z rotation in the scene</param>
        public JObject AddModelNode(string name, string parentUUID, string filePath, decimal xPosition = 0,
            decimal yPosition = 0, decimal zPosition = 0, decimal scale = 1, decimal xRotation = 0,
            decimal yRotation = 0, decimal zRotation = 0, bool cullBackFaces = true, bool animated = false,
            string animationPath = "")
        {
            SceneNodeAddDataAttributeComponentsTransform transform = new SceneNodeAddDataAttributeComponentsTransform()
            {
                Position = new decimal[] { xPosition, yPosition, zPosition }, Scale = scale,
                Rotation = new decimal[] { xRotation, yRotation, zRotation }
            };
            SceneNodeAddDataAttributeComponentsModel model = new SceneNodeAddDataAttributeComponentsModel()
                { File = filePath, CullBackFaces = cullBackFaces, Animated = animated, Animation = animationPath };
            SceneNodeAddDataAttributeComponents components = new SceneNodeAddDataAttributeComponents()
                { Transform = transform, Model = model };
            SceneNodeAddDataAttribute data = new SceneNodeAddDataAttribute()
                { Components = components, Name = name, Parent = parentUUID };
            SceneNodeAdd sceneNodeAdd = new SceneNodeAdd() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Adds a terrain to the scene
        /// </summary>
        /// <param _panelUUID="panelUUID">The _panelUUID of the terrain</param>
        /// <param _panelUUID="parentUUID">The parent of the terrain</param>
        /// <param _panelUUID="smoothNormals">false = normals are rendered lineairly, true = normals are rendered smoothly</param>
        /// <param _panelUUID="xPosition">x position in the scene</param>
        /// <param _panelUUID="yPosition">y position in the scene</param>
        /// <param _panelUUID="zPosition">z position in the scene</param>
        /// <param _panelUUID="scale">Scaling factor in the scene</param>
        /// <param _panelUUID="xRotation">x rotation in the scene</param>
        /// <param _panelUUID="yRotation">y rotation in the scene</param>
        /// <param _panelUUID="zRotation">z rotation in the scene</param>
        public JObject AddTerrainNode(string name = "Terrain", string parentUUID = null, decimal xPosition = 0,
            decimal yPosition = 0, decimal zPosition = 0, decimal scale = 1, decimal xRotation = 0,
            decimal yRotation = 0, decimal zRotation = 0, bool smoothNormals = true)
        {
            SceneNodeAddDataAttributeComponentsTransform transform = new SceneNodeAddDataAttributeComponentsTransform()
            {
                Position = new decimal[] { xPosition, yPosition, zPosition }, Scale = scale,
                Rotation = new decimal[] { xRotation, yRotation, zRotation }
            };
            SceneNodeAddDataAttributeComponentsTerrain terrain = new SceneNodeAddDataAttributeComponentsTerrain()
                { SmoothNormals = smoothNormals };
            SceneNodeAddDataAttributeComponents components = new SceneNodeAddDataAttributeComponents()
                { Transform = transform, Terrain = terrain };
            SceneNodeAddDataAttribute data = new SceneNodeAddDataAttribute()
                { Components = components, Name = name, Parent = parentUUID };
            SceneNodeAdd sceneNodeAdd = new SceneNodeAdd() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Adds a panel to the scene
        /// </summary>
        /// <param _panelUUID="panelUUID">The _panelUUID of the panel</param>
        /// <param _panelUUID="xSize">The x size of the panel</param>
        /// <param _panelUUID="ySize">The y size of the panel</param>
        /// <param _panelUUID="xResolution">The x resolution of the panel</param>
        /// <param _panelUUID="yResolution">The y resolution of the panel</param>
        /// <param _panelUUID="background"></param>
        /// <param _panelUUID="castShadow">false = shadows are not cast on and by the panel, true = shadows are cast on and by the panel</param>
        /// <param _panelUUID="parentUUID">The parent of the panel</param>
        /// <param _panelUUID="xPosition">x position in the scene</param>
        /// <param _panelUUID="yPosition">y position in the scene</param>
        /// <param _panelUUID="zPosition">z position in the scene</param>
        /// <param _panelUUID="scale">Scaling factor in the scene</param>
        /// <param _panelUUID="xRotation">x rotation in the scene</param>
        /// <param _panelUUID="yRotation">y rotation in the scene</param>
        /// <param _panelUUID="zRotation">z rotation in the scene</param>
        public JObject AddPanelNode(string name, string parentUUID, decimal xSize, decimal ySize, int xResolution,
            int yResolution, decimal[] background, bool castShadow = false, decimal xPosition = 0,
            decimal yPosition = 0, decimal zPosition = 0, decimal scale = 1, decimal xRotation = 0,
            decimal yRotation = 0, decimal zRotation = 0)
        {
            SceneNodeAddDataAttributeComponentsTransform transform = new SceneNodeAddDataAttributeComponentsTransform()
            {
                Position = new decimal[] { xPosition, yPosition, zPosition }, Scale = scale,
                Rotation = new decimal[] { xRotation, yRotation, zRotation }
            };
            SceneNodeAddDataAttributeComponentsPanel panel = new SceneNodeAddDataAttributeComponentsPanel()
            {
                Size = new decimal[] { xSize, ySize }, Resolution = new int[] { xResolution, yResolution },
                Background = background, CastShadow = castShadow
            };
            SceneNodeAddDataAttributeComponents components = new SceneNodeAddDataAttributeComponents()
                { Transform = transform, Panel = panel };
            SceneNodeAddDataAttribute data = new SceneNodeAddDataAttribute()
                { Components = components, Name = name, Parent = parentUUID };
            SceneNodeAdd sceneNodeAdd = new SceneNodeAdd() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Adds a body of water to the scene
        /// </summary>
        /// <param _panelUUID="panelUUID">The _panelUUID of the body of water</param>
        /// <param _panelUUID="xSize">The x size of the body of water</param>
        /// <param _panelUUID="ySize">The y size of the body of water</param>
        /// <param _panelUUID="resolution">The number of polygons used by the water (0.1 = 10000)</param>
        /// <param _panelUUID="parentUUID">The parent of the water</param>
        /// <param _panelUUID="xPosition">x position in the scene</param>
        /// <param _panelUUID="yPosition">y position in the scene</param>
        /// <param _panelUUID="zPosition">z position in the scene</param>
        /// <param _panelUUID="scale">Scaling factor in the scene</param>
        /// <param _panelUUID="xRotation">x rotation in the scene</param>
        /// <param _panelUUID="yRotation">y rotation in the scene</param>
        /// <param _panelUUID="zRotation">z rotation in the scene</param>
        public JObject AddWaterNode(string name, string parentUUID, decimal xSize, decimal ySize, decimal xPosition = 0,
            decimal yPosition = 0, decimal zPosition = 0, decimal scale = 1, decimal xRotation = 0,
            decimal yRotation = 0, decimal zRotation = 0, decimal resolution = 0.1m)
        {
            SceneNodeAddDataAttributeComponentsTransform transform = new SceneNodeAddDataAttributeComponentsTransform()
            {
                Position = new decimal[] { xPosition, yPosition, zPosition }, Scale = scale,
                Rotation = new decimal[] { xRotation, yRotation, zRotation }
            };
            SceneNodeAddDataAttributeComponentsWater water = new SceneNodeAddDataAttributeComponentsWater()
                { Size = new decimal[] { xSize, ySize }, Resolution = resolution };
            SceneNodeAddDataAttributeComponents components = new SceneNodeAddDataAttributeComponents()
                { Transform = transform, Water = water };
            SceneNodeAddDataAttribute data = new SceneNodeAddDataAttribute()
                { Components = components, Name = name, Parent = parentUUID };
            SceneNodeAdd sceneNodeAdd = new SceneNodeAdd() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Update a node
        /// </summary>
        /// <param name="uuid">UUID of the node</param>
        /// <param name="name">Name of the parent</param>
        public JObject UpdateNode(string uuid, string name)
        {
            int[] position = { 0, 0, 0 };
            int[] rotation = { 0, 90, 0 };
            SceneNodeUpdateTransformDataAttribute sceneNodeUpdateTransform = new SceneNodeUpdateTransformDataAttribute()
                { position = position, rotation = rotation, scale = 1 };

            SceneNodeUpdateDataAttribute data = new SceneNodeUpdateDataAttribute()
                { id = uuid, parent = name, Transform = sceneNodeUpdateTransform };
            SceneNodeUpdate sceneNodeUpdate = new SceneNodeUpdate() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeUpdate };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Deletes a node
        /// </summary>
        /// <param _panelUUID="nodeUUID">UUID of the node that needs to be deleted</param>
        public JObject DeleteNode(string nodeUUID)
        {
            SceneNodeDeleteDataAttribute data = new SceneNodeDeleteDataAttribute() { Id = nodeUUID };
            SceneNodeDelete sceneNodeDelete = new SceneNodeDelete() { Data = data };


            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeDelete };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }


        /// <summary>
        /// Adds a texture layer on top of a terrain
        /// </summary>
        /// <param _panelUUID="panelUUID">The _panelUUID of the terrain node</param>
        /// <param _panelUUID="diffusePath">The filepath of the diffuse of the texture</param>
        /// <param _panelUUID="normalPath">The filepath of the normalPath of the texture</param>w
        /// <param _panelUUID="minHeight">The minimum height from which the texture is visible</param>
        /// <param _panelUUID="maxHeight">The maximum height from which the texture is visible</param>
        /// <param _panelUUID="fadeDistance">The distance from the boundary (min and max) after which the texture is completly faded away</param>
        public JObject AddLayer(String name, String diffusePath, String normalPath, int minHeight, int maxHeight,
            int fadeDistance)
        {
            String id = FindUUIDByName(name);
            SceneNodeAddLayerDataAttribute data = new SceneNodeAddLayerDataAttribute()
            {
                Id = id, Normal = normalPath, Diffuse = diffusePath, MinHeight = minHeight, MaxHeight = maxHeight,
                FadeDist = fadeDistance
            };
            SceneNodeAddLayer sceneNodeAddlayer = new SceneNodeAddLayer() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeAddlayer };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Find a node from by its _panelUUID
        /// </summary>
        /// <param _panelUUID="nodeName">The _panelUUID of the node</param>
        /// <returns>The data of the node found</returns>
        public JObject FindNode(string nodeName)
        {
            SceneNodeFindDataAttribute data = new SceneNodeFindDataAttribute() { Name = nodeName };
            SceneNodeFind sceneNodeFind = new SceneNodeFind() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneNodeFind };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Find the UUID of a node from its _panelUUID
        /// </summary>
        /// <param _panelUUID="panelUUID">The _panelUUID of the node</param>
        /// <returns>The UUID of the node</returns>
        public string FindUUIDByName(string name)
        {
            if (name == null)
            {
                return null;
            }

            JObject jsonObject = FindNode(name);

            if (jsonObject == null)
            {
                return null;
            }

            return (string)jsonObject["data"]["data"]["data"][0]["uuid"];
        }

        /// <summary>
        /// Add a new terrain (this will replace the old terrain)
        /// </summary>
        /// <param _panelUUID="xSize">the x size of the terrain</param>
        /// <param _panelUUID="zSize">the z size of the terrain</param>
        /// <param _panelUUID="heightMap">a heightmap containing the height of each point on the map</param>
        public JObject AddTerrain(int xSize, int zSize, decimal[] heightMap)
        {
            SceneTerrainAddDataAttribute data = new SceneTerrainAddDataAttribute()
                { Size = new int[] { xSize, zSize }, Heights = heightMap };
            SceneTerrainAdd sceneTerrainAdd = new SceneTerrainAdd() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneTerrainAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };

            return Write(tunnelSend);
        }

        /// <summary>
        /// Get the height of the terrain of one or more positions
        /// </summary>
        /// <param _panelUUID="position">position to get the height of</param>
        /// <param _panelUUID="positions">positions to get the height of</param>
        /// <returns></returns>
        public decimal[] getTerrainHeights(decimal[] position, decimal[][] positions = null)
        {
            SceneTerrainGetHeightDataAttribute data;

            if (positions == null)
            {
                data = new SceneTerrainGetHeightDataAttribute() { Position = position, Positions = positions };
            }
            else
            {
                data = new SceneTerrainGetHeightDataAttribute() { Position = null, Positions = positions };
            }

            SceneTerrainGetHeight sceneTerrainGetHeight = new SceneTerrainGetHeight() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneTerrainGetHeight };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            JObject jsonObject = Write(tunnelSend);


            if (positions == null)
            {
                return new decimal[] { jsonObject["data"]["data"]["data"]["height"].Value<decimal>() };
            }
            else
            {
                return jsonObject["data"]["data"]["data"]["heights"].Value<JArray>().Select(h => h.Value<decimal>())
                    .ToArray();
            }
        }

        /// <summary>
        /// Clears the contents of the panel to the background color. 
        /// Should be used before drawing anything on it. 
        /// </summary>
        /// <param _panelUUID="panelUUID">UUID of the node that the panel is on</param>
        public JObject ClearPanel(string panelUUID)
        {
            ScenePanelClearDataAttribute data = new ScenePanelClearDataAttribute() { Id = panelUUID };
            ScenePanelClear scenePanelClear = new ScenePanelClear() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = scenePanelClear };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Draws multiple multiple lines on the the backbuffer for this panel. 
        /// Lines can have different colors, but all share the same width. _panelUUID should be the _panelUUID of the node of the panel
        /// </summary>
        /// <param _panelUUID="panelUUID">UUID of the node that the panel is on</param>
        /// <param _panelUUID="width">Width of the line</param>
        /// <param _panelUUID="lines">Array of nested Arrays with points of the lines that should be drawn</param>
        public JObject AddPanelLines(string panelUUID, int width, int[][] lines)
        {
            ScenePanelDrawLinesDataAttribute data = new ScenePanelDrawLinesDataAttribute()
                { Id = panelUUID, Width = width, Lines = lines };
            ScenePanelDrawLines scenePanelDrawLines = new ScenePanelDrawLines() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = scenePanelDrawLines };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Draws text on panel on specified location
        /// </summary>
        /// <param _panelUUID="panelUUID">UUID of the node that the panel is on</param>
        /// <param _panelUUID="text">Text that will be displayed</param>
        /// <param _panelUUID="position">Position of the text</param>
        /// <param _panelUUID="size">Size of the text</param>
        /// <param _panelUUID="color">Array with RGB and alpha values for colors</param>
        /// <param _panelUUID="font">Name of the font the text should be displayed in</param>
        public JObject AddPanelText(string paneUUID, string text, decimal[] position, decimal size, decimal[] color,
            string font)
        {
            ScenePanelDrawTextDataAttribute data = new ScenePanelDrawTextDataAttribute()
                { Id = paneUUID, Text = text, Position = position, Size = size, Color = color, Font = font };
            ScenePanelDrawText scenePanelDrawText = new ScenePanelDrawText() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = scenePanelDrawText };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Draws an image on the panel. Image must be available on the NetworkEngine
        /// </summary>
        /// <param _panelUUID="panelUUID">UUID of the node that the panel is on</param>
        /// <param _panelUUID="imageFilePath">The file path to the image that should be displayed</param>
        /// <param _panelUUID="position">The position of the image</param>
        /// <param _panelUUID="size">The size the image should be</param>
        public JObject AddPanelImage(string panelUUID, string imageFilePath, decimal[] position, decimal[] size)
        {
            ScenePanelImageDataAttribute data = new ScenePanelImageDataAttribute()
                { Id = panelUUID, Image = imageFilePath, Position = position, Size = size };
            ScenePanelImage scenePanelImage = new ScenePanelImage() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = scenePanelImage };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Changes the clear color. Color values are in the range of 0 - 1 (0,0,0 is black, 1,1,1 is white),
        /// fourth value is alpha value, 0 is transparent, 1 is opaque
        /// </summary>
        /// <param _panelUUID="panelUUID">Name of the node the panel is on</param>
        /// <param _panelUUID="color">Array with RGB and alpha values for color</param>
        public JObject SetPanelClearColor(string panelUUID, decimal[] color)
        {
            ScenePanelSetClearColorDataAttribute data = new ScenePanelSetClearColorDataAttribute()
                { Id = panelUUID, Color = color };
            ScenePanelSetClearColor scenePanelSetClearColor = new ScenePanelSetClearColor() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = scenePanelSetClearColor };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Swaps the buffers for this panel. id should be the uuid of the node of the panel to be cleared
        /// </summary>
        /// <param _panelUUID="panelUUID">Name of the node the panel is on</param>
        public JObject SwapPanel(string panelUUID)
        {
            ScenePanelSwapDataAttribute data = new ScenePanelSwapDataAttribute() { Id = panelUUID };
            ScenePanelSwap scenePanelSwap = new ScenePanelSwap() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = scenePanelSwap };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }


        /// <summary>
        /// Set the time of day in VR
        /// </summary>
        /// <param _panelUUID="time">The time of day. 12 = 12:00 23 = 23:00 and 14.5 = 14:30</param>
        public JObject SetTime(decimal time)
        {
            SkyBoxSetTimeDataAttribute data = new SkyBoxSetTimeDataAttribute() { Time = time };
            SkyBoxSetTime skyBoxSetTime = new SkyBoxSetTime() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = skyBoxSetTime };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Add a road texture to the route
        /// </summary>
        /// <param _panelUUID="UUID">The UUID of the route</param>
        /// <param _panelUUID="diffusePath">The path of the diffuse texture</param>
        /// <param _panelUUID="normalPath">The path of the normal map</param>
        /// <param _panelUUID="specularPath">The path of the specular data</param>
        /// <param _panelUUID="heightOffset">The offset compared to the ground (to avoid clipping)</param>
        public JObject AddRoad(string UUID, string diffusePath, string normalPath, string specularPath,
            decimal heightOffset = 0.1m)
        {
            SceneRoadAddDataAttribute data = new SceneRoadAddDataAttribute()
            {
                Route = UUID, Diffuse = diffusePath, Normal = normalPath, Specular = specularPath,
                HeightOffset = heightOffset
            };
            SceneRoadAdd sceneRoadAdd = new SceneRoadAdd { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = sceneRoadAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Add a route to the scene
        /// </summary>
        /// <param _panelUUID="nodes">A list of nodes with a position and vector</param>
        /// <returns>The UUID of the added route</returns>
        public String AddRoute(RouteAddDataAttributeNode[] nodes)
        {
            RouteAddDataAttribute data = new RouteAddDataAttribute() { Nodes = nodes };
            RouteAdd routeAdd = new RouteAdd() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = routeAdd };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            JObject jsonObject = Write(tunnelSend);
            return (string)jsonObject["data"]["data"]["data"]["uuid"];
        }

        public enum RouteRotation
        {
            None,
            XZ,
            XYZ
        }

        /// <summary>
        /// Makes a node follow a route
        /// </summary>
        /// <param _panelUUID="routeUUID">The UUID of the route</param>
        /// <param _panelUUID="nodeUUID">The UUID of the node</param>
        /// <param _panelUUID="speed">The speed at wich the node should travel</param>
        /// <param _panelUUID="offset">The offset compared to the route</param>
        /// <param _panelUUID="rotate">NONE = the model should not rotate with the route, XZ = the model should rotate on the XZ axis with the route, XYZ = the model should rotate with the route</param>
        /// <param _panelUUID="smoothing">The amount of smoothing to follow the route</param>
        /// <param _panelUUID="followHeight">The height offset compared to the route</param>
        /// <param _panelUUID="rotateOffset">The rotation offset compared to the route</param>
        /// <param _panelUUID="positionOffset">The position offset compared to the route</param>
        public JObject FollowRoute(string routeUUID, string nodeUUID, decimal speed, decimal offset,
            RouteRotation rotate, decimal smoothing, bool followHeight, decimal[] rotateOffset,
            decimal[] positionOffset)
        {
            string rotation;

            switch (rotate)
            {
                case RouteRotation.None:
                    rotation = "NONE";
                    break;
                case RouteRotation.XZ:
                    rotation = "XZ";
                    break;
                case RouteRotation.XYZ:
                    rotation = "XYZ";
                    break;
                default:
                    rotation = "NONE";
                    break;
            }

            RouteFollowDataAttribute data = new RouteFollowDataAttribute()
            {
                Route = routeUUID, Node = nodeUUID, Speed = speed, Offset = offset, Rotate = rotation,
                Smoothing = smoothing, FollowHeight = followHeight, RotateOffset = rotateOffset,
                positionOffset = positionOffset
            };
            RouteFollow routeFollow = new RouteFollow { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = routeFollow };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Changed the speed of a node on a route
        /// </summary>
        /// <param _panelUUID="nodeId">Id of the node on the route</param>
        /// <param _panelUUID="speed">the new speed for selected node on route</param>
        public JObject FollowRouteSpeed(String nodeId, decimal speed)
        {
            RouteFollowSpeedDataAttribute data = new RouteFollowSpeedDataAttribute() { Node = nodeId, Speed = speed };
            RouteFollowSpeed routeFollowSpeed = new RouteFollowSpeed { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = routeFollowSpeed };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// shows or hides all routes (the red line). If show parameter is not set, it toggles the visibility
        /// </summary>
        /// <param _panelUUID="show"> true to show route, false to hide route</param>
        public JObject showRoute(bool show)
        {
            RouteShowDataAttribute data = new RouteShowDataAttribute() { Show = show };
            RouteShow routeShow = new RouteShow() { Data = data };

            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = routeShow };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Sets the engine to run if paused.
        /// </summary>
        public JObject Play()
        {
            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = new Play() };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }

        /// <summary>
        /// Pauses the rendering engine. Will make the screen turn dark
        /// </summary>
        public JObject Pause()
        {
            TunnelSendDataAttribute tunnelSendDataAttribute = new TunnelSendDataAttribute()
                { Dest = _session, Data = new Pause() };
            TunnelSend tunnelSend = new TunnelSend() { Data = tunnelSendDataAttribute };
            return Write(tunnelSend);
        }
    }
}