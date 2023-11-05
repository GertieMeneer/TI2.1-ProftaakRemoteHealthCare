namespace TI2._1_HealthCareServer.ClientConnection.Commands.StatusMessage
{
    public class StatusError : IStatusMessage
    {
        public string Status => "error";
        public string Error { get; set; }
    }
}