using TI2._1_HealthCareClient.BLEConnection.Translator;

namespace TI2._1_HealthCareClient.BLEConnection.Devices
{
    /// <summary>
    /// Interface to specify methods for FitnessBLE and the Emulators
    /// </summary>
    public interface IBLEDevice
    {
        event OnDataReceive DataReceived;
        void Connect();
        void Disconnect();
        void Read(byte[] data);
        void Write(byte[] sent);
    }
}
