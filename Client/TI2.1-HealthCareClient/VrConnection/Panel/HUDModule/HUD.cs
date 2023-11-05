using System;

namespace TI2._1_HealthCareClient.VRConnection.Panel.HUDModule
{
    /// <summary>
    /// Class responsible for saving the HUD data
    /// </summary>
    public class HUD
    {
        public DateTime DateTime { get; set; }
        public decimal Speed { get; set; }

        public int HeartRate { get; set; }
    }
}
