using System.Collections.Generic;
using System.Linq;

namespace TI2._1_HealthCareClient.VRConnection.Panel.ChatModule
{
    /// <summary>
    /// Class responsible for saving the VR chat
    /// </summary>
    public class Chat
    {
        public List<ChatMessage> Messages { get; } = new List<ChatMessage>();
        public int MaximumCharacterLineCount { get; }
        public int MaximumLineCount { get; }

        /// <summary> 
        /// Create a new chat
        /// </summary>
        /// <param name="maximumCharacterLineCount">The maximum number of characters that can be displayed on a line</param>
        public Chat(int maximumCharacterLineCount, int maximumLineCount)
        {
            MaximumCharacterLineCount = maximumCharacterLineCount;
            MaximumLineCount = maximumLineCount;
        }

        /// <summary>
        /// Remove all chat messages from the chat
        /// </summary>
        public void RemoveAllmessages()
        {
            Messages.Clear();
        }

        /// <summary>
        /// Add a message to the chat
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="sender">The sender of the message</param>
        /// <param name="messageType">The message type (system, global or doctor)</param>
        public void AddMessage(string sender, string message, MessageType messageType)
        {
            Messages.Add(new ChatMessage(SplitMessage(message),sender, messageType));
        }

        /// <summary>
        /// Splits a message into a number of lines, based on the maximum length of characters on a single line
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The message split into seperate lines seperated by a new line character</returns>
        private string SplitMessage(string message)
        {
            string[] words = message.Split(' ');
            string splitMessage = "";

            int lineLenght = 0;

            foreach (string word in words)
            {
                lineLenght += word.Length;

                if (lineLenght > MaximumCharacterLineCount)
                {
                    lineLenght = word.Length;
                    splitMessage += "\n" + word + " ";
                }
                else
                {
                    splitMessage += word + " ";
                    lineLenght++;
                }
            }
            return splitMessage;
        }
    }
}