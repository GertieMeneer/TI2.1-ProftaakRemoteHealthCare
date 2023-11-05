using TI2._1_HealthCareClient.Commands;

namespace TI2._1_HealthCareClient.VrConnection.Commands.General
{
    public class SessionList : IVRCommand
    {
        public string Id { get { return "session/list"; } }
    }
}
