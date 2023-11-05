using TI2._1_HealthCareServer.ClientConnection.Commands.BikeSetresistance.DataAttributes;

namespace TI2._1_HealthCareServer.ClientConnection.Commands.BikeSetresistance
{
    public class BikeSetResistance : IClientCommand
    {
        public string Command = "bike/setresistance";
        public BikeSetResistanceData Data { set; get; }
    }
}
