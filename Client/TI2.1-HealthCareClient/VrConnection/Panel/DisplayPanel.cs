using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TI2._1_HealthCareClient.VRConnection.Panel.ChatModule;
using TI2._1_HealthCareClient.VRConnection.Panel.HUDModule;

namespace TI2._1_HealthCareClient.VRConnection.Panel
{
    public class DisplayPanel
    {
        private VrClientConnection _vrClientConnection = VrClientConnection.GetInstance();

        private string _PanelUUID;
        private Chat _chatModule;
        private HUD _hudModule;

        /// <summary>
        /// Creates a panel on the bike
        /// </summary>
        /// <param name="panelName">The name of the panel</param>
        /// /// <param name="modelName">The name of the chatboard model</param>
        /// <param name="parentName">The panelName of the parent</param>
        public DisplayPanel(string panelName, string modelName, string parentName)
        {
            // Find the root directory (to be used to load in resources)
            string dir = Directory.GetCurrentDirectory();
            string root = dir.Substring(0, dir.Length - 9) + @"Resources\";

            // Add the panel model
            _vrClientConnection.AddModelNode(modelName, _vrClientConnection.FindUUIDByName(parentName), root + @"Panel\Models\Speedometer.fbx", -0.6m, 1m, 0, 0.03m, 0, 0, -35);
            _vrClientConnection.AddPanelNode(panelName, _vrClientConnection.FindUUIDByName(modelName), 9, 14, 500, 500, new decimal[] { 0, 0, 0, 0 }, false, -7m, 2.75m, 0, 1, 0, 90, 90);
            _PanelUUID = _vrClientConnection.FindUUIDByName(panelName);

            // add the chat
            _chatModule = new Chat(25, 11);

            // add the HUD
            _hudModule = new HUD();
        }

        /// <summary>
        /// clears the chat
        /// </summary>
        public void ClearChat()
        {
            _chatModule.RemoveAllmessages();
        }

        /// <summary>
        /// Add a message to the chat
        /// </summary>
        /// <param name="sender">The name of the sender</param>
        /// <param name="message">The content of the message</param>
        /// <param name="messageType">The message type (system, global, doctor)</param>
        public void addMessage(string sender, string message, MessageType messageType)
        {
            _chatModule.AddMessage(message, sender, messageType);
        }

        /// <summary>
        /// Set the speed of the display
        /// </summary>
        /// <param name="speed"></param>
        public void setSpeed(decimal speed)
        {
            _hudModule.Speed = speed;
        }

        public void SetHeartRate(int heartRate)
        {
            _hudModule.HeartRate = heartRate;
        }

        /// <summary>
        /// Update the entire panel
        /// </summary>
        public void updatePanel()
        {
            // Clear the panel
            _vrClientConnection.ClearPanel(_PanelUUID);

            // Draw the outlines
            int[] topLine = new int[] { 0, 60, 500, 60, 1, 1, 1, 1};

            _vrClientConnection.AddPanelLines(_PanelUUID, 2, new int[][] { topLine});

            // Update all components
            UpdateTime();
            UpdateSpeed();
            // UpdateHeartRate();
            UpdateChat();


            // Display components
            _vrClientConnection.SwapPanel(_PanelUUID);
        }

        /// <summary>
        /// Update the speed on the display.
        /// </summary>
        private void UpdateSpeed()
        {
            _vrClientConnection.AddPanelText(_PanelUUID, String.Format("{0:00.0} km/h", Math.Round(_hudModule.Speed, 1)), new decimal[] { 20, 50}, 30, new decimal[] { 1, 1, 1, 1 }, @"Calibri");
        }

        private void UpdateTime() {
            _vrClientConnection.AddPanelText(_PanelUUID, DateTime.Now.ToString("H:mm"), new decimal[] { 400, 50 }, 30, new decimal[] { 1, 1, 1, 1 }, @"Calibri");
        }

        private void UpdateHeartRate()
        {
            _vrClientConnection.AddPanelText(_PanelUUID, _hudModule.HeartRate + " BPM", new decimal[] { 200, 50 }, 30, new decimal[] { 1, 1, 1, 1 }, @"Calibri");
        }

        /// <summary>
        /// Update the chat in Vr
        /// </summary>
        private void UpdateChat()
        {
            List<ChatMessage> messages = _chatModule.Messages;
            int chatLineCount = CalculateChatLineCount() + messages.Count;

            // Remove chat messages until the messages fit the panel
            while (chatLineCount > _chatModule.MaximumLineCount)
            {
                chatLineCount -= messages[0].Message.Split('\n').Length;
                messages.RemoveAt(0);
            }

            int lineNumber = 0;
            int messageNumber = 0;

            // Add each message to the panel
            foreach (ChatMessage chatMessage in messages)
            {
                (decimal[], decimal[]) colors = MessageColor(chatMessage.MessageType);
                _vrClientConnection.AddPanelText(_PanelUUID, chatMessage.Sender + ":", new decimal[] { 20, 90 + lineNumber * 40 }, 40, colors.Item1, "Calibri");
                
                lineNumber++;
                messageNumber++;

                string[] lines = chatMessage.Message.Split('\n');

                foreach (string line in lines)
                {
                    _vrClientConnection.AddPanelText(_PanelUUID, line, new decimal[] { 20, 90 + lineNumber * 40 }, 40, colors.Item2, "Calibri");
                    lineNumber++;
                }
            }
        }

        /// <summary>
        /// Calculate the total number of lines in the chat
        /// </summary>
        /// <returns></returns>
        private int CalculateChatLineCount()
        {
            List<ChatMessage> messages = _chatModule.Messages;

            int lineCount = 0;

            foreach (ChatMessage chatMessage in messages)
            {
                lineCount += chatMessage.Message.Split('\n').Length;
            }
            return lineCount;
        }

        /// <summary>
        /// Get the color for the message
        /// </summary>
        /// <param name="messageType">The type of the message (system, global or doctor)</param>
        /// <returns>The color that should be used for the sender and the color that should be used for the message (default is black)</returns>
        private (decimal[] senderColor, decimal[] chatColor) MessageColor(MessageType messageType)
        {
            decimal[] chatColor = new decimal[] { 1, 1, 1, 1 };
            decimal[] senderColor = new decimal[] { 1, 1, 1, 1 };

            switch (messageType)
            {
                case MessageType.System:
                    senderColor = new decimal[] { 1, 0.65m, 0, 1 };
                    chatColor = new decimal[] { 1, 0.65m, 0, 1 };
                    break;
                case MessageType.Global:
                    senderColor = new decimal[] { 1, 0, 0, 1 };
                    break;
                case MessageType.Doctor:
                    senderColor = new decimal[] { 0, 0, 1, 1 };
                    break;
            }

            return (senderColor, chatColor);
        }
    }
}
