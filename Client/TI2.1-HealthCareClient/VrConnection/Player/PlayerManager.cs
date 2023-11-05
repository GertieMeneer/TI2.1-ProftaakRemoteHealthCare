using System;
using System.IO;

namespace TI2._1_HealthCareClient.VRConnection.Player
{
    /// <summary>
    /// Class responsible for controlling the player
    /// </summary>
    public class PlayerManager
    {
        private VrClientConnection _vrClientConnection = VrClientConnection.GetInstance();
        private string _vehicleUUID;

        /// <summary>
        /// Setup the player manager
        /// </summary>
        /// <param name="vechicleName"></param>
        public PlayerManager(string vechicleName) { 
            _vehicleUUID = _vrClientConnection.FindUUIDByName(vechicleName);
        }

        /// <summary>
        /// Set the speed of the player to a different speed
        /// </summary>
        /// <param name="speed">The speed of the player in km/h</param>
        public void SetPlayerSpeed(decimal speed)
        {
            _vrClientConnection.FollowRouteSpeed(_vehicleUUID, speed);
        }

        /// <summary>
        /// Start a session
        /// </summary>
        public void Start()
        {
            // RemoveRoadBLock();
            // Send system message that says the session has started
        }

        /// <summary>
        /// Stop a session
        /// </summary>
        public void Stop()
        {
            SetPlayerSpeed(0);
            // Clear the player chat
            // AddRoadBLock("     Session has\n         stopped");
            // Add system message that you should scan you pass to identify yourself
        }

        /// <summary>
        /// Pause the session
        /// </summary>
        public void Pause()
        {
            _vrClientConnection.Pause();
            SetPlayerSpeed(0);
            AddRoadBLock("Session has paused");
            // Send system message that the session has paused
        }

        /// <summary>
        /// Resume the session
        /// </summary>
        public void Resume()
        {
            RemoveRoadBLock();
            _vrClientConnection.Play();
            // Send system message that the session has resumed
        }

        private void AddRoadBLock(string message)
        {
            // Find the root directory
            string dir = Directory.GetCurrentDirectory();
            string root = dir.Substring(0, dir.Length - 9) + @"Resources\";

            _vrClientConnection.AddModelNode("Roadblock", _vehicleUUID, root + @"Scene\Roadblock.fbx", -3);
            _vrClientConnection.AddPanelNode("RoadblockPanel", _vrClientConnection.FindUUIDByName("Roadblock"), 1.5m, 0.75m, 750, 500, new decimal[] { 0, 0, 0, 0 }, false, 0.1m, 1.85m, 0.7m, 1, 0, 100, 0);
            string panelUUID = _vrClientConnection.FindUUIDByName("RoadblockPanel");

            _vrClientConnection.ClearPanel(panelUUID);

            string[] lines = message.Split('\n');
            int i = 0;

            foreach (string line in lines)
            {
                _vrClientConnection.AddPanelText(panelUUID, line, new decimal[] { 20, 200 + i * 100 }, 100, new decimal[] { 1, 1, 1, 1 }, "Calibri");
                i++;
            }
            _vrClientConnection.SwapPanel(panelUUID);
        }

        private void RemoveRoadBLock()
        {
            try
            {
                _vrClientConnection.DeleteNode(_vrClientConnection.FindUUIDByName("Roadblock"));
                _vrClientConnection.DeleteNode(_vrClientConnection.FindUUIDByName("RoadblockPanel"));
            }
            catch (Exception)
            {

            }
            
        }
    }
}
