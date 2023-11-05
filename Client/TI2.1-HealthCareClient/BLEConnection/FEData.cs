namespace TI2._1_HealthCareClient {
    public class FEData {
        
        // generic data
        public string TimeStamp { get; set; } = "2000-00-00 00:00:00.000";
        public string DataType { get; set; } = "Unknown";
        public byte DataPage { get; set; } = 0xFF;
        
        
        // General FE Transform
        public double Timer { get; set; } = double.NaN;
        public double Distance { get; set; } = double.NaN;
        public double Speed { get; set; } = double.NaN;
        
        
        // Specific Trainer/Stationary Bike Transform
        public double Cadence { get; set; } = double.NaN;
        public int TotalPower { get; set; }
        public int CurrentPower { get; set; }
        
        
        // Heart Rate Transform
        public int HeartRate { get; set; }
    }
}