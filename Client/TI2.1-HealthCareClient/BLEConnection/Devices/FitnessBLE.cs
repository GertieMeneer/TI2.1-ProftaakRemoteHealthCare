using System;
using System.Text;
using Avans.TI.BLE;

namespace TI2._1_HealthCareClient.BLEConnection.Devices
{
    public delegate void OnDataReceive(byte[] data, DeviceType deviceType);
    /// <summary>
    /// This class is responsible for connecting to BLE devices using the given BLE library.
    /// It also subscribes to an event, so when data is received from the BLE device, subscribers will be able to receive that data instantly.
    /// </summary>
    public class FitnessBLE : IBLEDevice
    {
        public event OnDataReceive DataReceived;
        public DeviceType DeviceType { get; }

        private BLE ble = new BLE();
        private String serialNumber;

        /// <summary>
        /// For connecting BLE devices that don't require a serialnumber
        /// </summary>
        /// <param _panelUUID="device">Type of device that needs to be connected</param>
        public FitnessBLE(DeviceType deviceType) : this(null, deviceType) { }

        /// <summary>
        /// Used for connecting bicycle.
        /// </summary>
        /// <param _panelUUID="serialNumber">Used to connect with a specific bicycle</param>
        /// <param _panelUUID="device">Type of device that needs to be connected</param> 
        public FitnessBLE(String serialNumber, DeviceType deviceType)
        {
            //Subscribe to BLE function so that new data goes to the function
            ble.SubscriptionValueChanged += incomingData;
            this.serialNumber = serialNumber;
            this.DeviceType = deviceType;
        }

        /// <summary>
        /// Function used for receiving data and passing it to the read function.
        /// </summary>
        /// <param _panelUUID="Sender">The BLE device which sends the data</param>
        /// <param _panelUUID="e">The data itself</param>
        private void incomingData(object Sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] bytes = e.Data;
            Read(bytes);
        }

        /// <summary>
        /// Used for connecting to a fitness device.
        /// </summary>
        /// <exception cref="Exception">When an unknown device is being connected</exception>
        public async void Connect()
        {
            switch (DeviceType)
            {
                case DeviceType.Bike:
                    await ble.OpenDevice("Tacx Flux " + serialNumber);
                    await ble.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
                    await ble.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
                    break;
                case DeviceType.HeartrateSensor:
                    await ble.OpenDevice("Decathlon Dual HR");
                    await ble.SetService("HeartRate");
                    await ble.SubscribeToCharacteristic("HeartRateMeasurement");
                    break;
                default:
                    throw new Exception("Device not found");
            }
        }

        /// <summary>
        /// Disconnecting BLE devie
        /// </summary>
        public void Disconnect()
        {
            ble.CloseDevice();
        }

        /// <summary>
        /// Transform comes in and gets passed to the event
        /// </summary>
        /// <param _panelUUID="data">The data in a byte array which is being passed through</param>
        public void Read(byte[] data)
        {
            DataReceived(data, DeviceType);
        }

        /// <summary>
        /// Used for sending data to the BLE device
        /// </summary>
        /// <param _panelUUID="sent">the data in a byte array that has to be send</param>
        public async void Write(byte[] sent)
        {
            await ble.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", sent);
        }

        /// <summary>
        /// Test method for testing the event and delegate
        /// </summary>
        public void translatorNotifyTest()
        {
            string testData = "Dit is een test";
            byte[] testBytes = Encoding.UTF8.GetBytes(testData);
            DataReceived.Invoke(testBytes, DeviceType);
        }
    }
}
