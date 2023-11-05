using System;
using System.Collections.Generic;
using TI2._1_HealthCareClient.BLEConnection.Devices;

namespace TI2._1_HealthCareClient.BLEConnection.Translator
{
    public delegate void DataTranslated(FEData feData);

    /// <summary>
    /// This class is used to translate the data from the BLE device to a FEData object
    /// </summary>
    public class Translator
    {
        public List<IBLEDevice> Devices { get; } = new List<IBLEDevice>();
        private BicycleTranslator _bicycleTranslator = new BicycleTranslator();
        private HeartRateMonitorTranslator _heartRateMonitorTranslator = new HeartRateMonitorTranslator();
        public event DataTranslated DataTranslated;

        /// <summary>
        /// Add a BLE device to be translated when it sends data
        /// </summary>
        /// <param _panelUUID="device">The BLE device that should be added</param>
        public void AddBlEDevice(IBLEDevice device)
        {
            device.DataReceived += OnDataReceived;
            Devices.Add(device);
        }

        /// <summary>
        /// This method is used to receive the data from the BLE device and translate it to a FEData object
        /// then it invokes the DataTranslated event
        /// </summary>
        /// <param _panelUUID="data"></param>
        private void OnDataReceived(byte[] data, DeviceType deviceType)
        {
            var feData = new FEData();
            ITranslatorStrategy translator = null;
            bool isValidDevice = true;

            switch (deviceType)
            {
                case DeviceType.HeartrateSensor:
                    feData = _heartRateMonitorTranslator.Translate(data);
                    break;
                case DeviceType.Bike:
                    feData = _bicycleTranslator.Translate(data);
                    break;
                default:
                    Console.WriteLine("not an valid byte array" + BitConverter.ToString(data));
                    isValidDevice = false;
                    break;
            }

            if (isValidDevice)
            {
                DataTranslated?.Invoke(feData);
            }
        }
    }
}

/// <summary>
/// different data pages of the bicycle
/// </summary>
public enum DataPages
{
    GeneralFeData = 0x10,
    SpecificTrainerOrStationaryBikeData = 0x19
}