namespace TI2._1_HealthCareClient.VRConnection.Panel.ChatModule
{
    /// <summary>
    /// Enumeration for defining the message type
    /// </summary>
    public enum MessageType
    {
        System, Global, Doctor
    }

    /// <summary>
    /// Class responsible for storing the data of a single chat message
    /// </summary>
    public class ChatMessage
    {
        public string Sender { get; }
        public string Message { get; }
        public MessageType MessageType { get; }

        /// <summary>
        /// Create a new message
        /// </summary>
        /// <param name="sender">The name of the sender</param>
        /// <param name="message">The message content</param>
        /// <param name="messageType">The type of the message (global, system or doctor)</param>
        public ChatMessage(string message, string sender, MessageType messageType)
        {
            Sender = sender;
            Message = message;
            MessageType = messageType;
        }
    }
}
