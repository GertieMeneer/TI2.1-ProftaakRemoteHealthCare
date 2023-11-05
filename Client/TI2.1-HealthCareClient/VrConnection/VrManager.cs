using System;
using System.Diagnostics;
using System.IO;
using TI2._1_HealthCareClient.VRConnection.Panel.ChatModule;
using TI2._1_HealthCareClient.VRConnection.Player;
using TI2._1_HealthCareClient.VRConnection.World.Scenes;

namespace TI2._1_HealthCareClient.VRConnection
{
    /// <summary>
    /// Main interface with the VR
    /// </summary>
    public class VrManager
    {
        private bool _isDebugMode;
        public bool DebugMode { get { return _isDebugMode; } set { _isDebugMode = value; VrClientConnection.GetInstance().DebugMode = value; } }

        private Process _networkEngine;
        public delegate void NetworkEngineClosedEventHandler(object sender, EventArgs e);
        public event NetworkEngineClosedEventHandler NetworkEngineClosed;

        private Scene _scene;
        private PlayerManager _player;
        private Panel.DisplayPanel _displayPanel;

        /// <summary>
        /// Sets up a connection to the VR server and initialize VR world
        /// </summary>
        /// <param _panelUUID="ip">The IP of the VR server</param>
        /// <param _panelUUID="port">The port number of the VR server</param>
        /// <param _panelUUID="deviceName">The device on which the VRClient is running</param>
        public void Connect(string ip, int port, string deviceName)
        {
            try
            {
                VrClientConnection.GetInstance().Connect(ip, port, deviceName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
            }
        }

        /// <summary>
        /// Disconnect from the Vr server
        /// </summary>
        public void Disconnect()
        {
            VrClientConnection.GetInstance().Disconnect();
        }

        /// <summary>
        /// Open the Network Engine
        /// </summary>
        public void OpenNetworkEngine(string fileName)
        {
            // Find the root directory
            string dir = Directory.GetCurrentDirectory();
            string batchFilePath = dir.Substring(0, dir.Length - 9) + @"NetworkEngine\";
            
            if (File.Exists(batchFilePath + fileName))
            {
                _networkEngine = new Process();
                _networkEngine.StartInfo.WorkingDirectory = batchFilePath;
                _networkEngine.StartInfo.FileName = fileName;
                _networkEngine.StartInfo.CreateNoWindow = false;
            
                _networkEngine.EnableRaisingEvents = true;
                _networkEngine.Exited += (sender, e) => OnNetworkEngineClosed();
            
                _networkEngine.Start();
            }
            else
            {
                Console.WriteLine("The specified .bat file does not exist.");
            }
        }

        /// <summary>
        /// Event that triggers when the Network Engine is closed
        /// </summary>
        private void OnNetworkEngineClosed()
        {
            // Raise the event to notify subscribers that the application has been closed
            NetworkEngineClosed?.Invoke(this, EventArgs.Empty);

            // Disconnect from the VR server if still connected
            Disconnect();
        }

        public void LoadScene(string sceneName, bool isFirstTimeSetup, bool useSimpleScene)
        {
            // Create the scene
            _scene = new FarmScene(sceneName, isFirstTimeSetup, useSimpleScene);

            // Add the bike
            _player = new PlayerManager("Vehicle");

            // Add the panels
            _displayPanel = new Panel.DisplayPanel("Panel", "DisplayModel", "Vehicle");
        }

        /// <summary>
        /// Start a new session in VR
        /// </summary>
        public void StartSession()
        {
            _player.Start();
        }

        /// <summary>
        /// Stop a session in VR
        /// </summary>
        public void StopSession()
        {
            _player.Stop();
        }

        /// <summary>
        /// Pause a session in VR
        /// </summary>
        public void pauseSession()
        {
            _player.Pause();
        }

        /// <summary>
        /// Resume a session in Vr
        /// </summary>
        public void resumeSession()
        {
            _player.Resume();
        }

        /// <summary>
        /// Set the speed of the player in VR and update the panel
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(decimal speed)
        {
            _displayPanel.setSpeed(speed);
            _displayPanel.updatePanel();

            _player.SetPlayerSpeed(speed);
        }

        public void SetHeartRate(int heartRate)
        {
            // _displayPanel.SetHeartRate(heartRate);
            // _displayPanel.updatePanel();
        }

        /// <summary>
        /// Send a chat message to the patient on the panel
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="sender">The name of the sender</param>
        /// <param name="messageType">The type of message (system, global, doctor)</param>
        public void SendChatMessage(string sender, string message, MessageType messageType)
        {
            _displayPanel.addMessage(sender, message, messageType);
            _displayPanel.updatePanel();
        }
    }
}
