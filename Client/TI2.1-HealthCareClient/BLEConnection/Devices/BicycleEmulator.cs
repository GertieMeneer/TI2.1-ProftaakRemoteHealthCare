using System;
using System.Threading;
using TI2._1_HealthCareClient.BLEConnection.Translator;

namespace TI2._1_HealthCareClient.BLEConnection.Devices
{
    /// <summary>
    /// Class that emulates a bicycle using the ANT+ protocol
    /// </summary>
    public class BicycleEmulator : IBLEDevice
    {
        //these bytes are constant in the data array
        private const byte _SYNCByte = 0xA4;
        private const byte _lenghtByte = 0x09;
        private const byte _dataTypeByte = 0x4E;
        private const byte _channelByte = 0x05;

        //variables for page 25
        public byte _targetRpm { get; set; } // holds the value for target rpm in RPM
        public ushort InstantWattage {get { return _instantWattage; } set { if (value > 4095) { throw new ArgumentOutOfRangeException(nameof(value));}_instantWattage = value;}}
        private ushort _instantWattage; // holds the value for wattage generated at an interval in Watt 
        private ushort _accumulatedWattage = 0; // holds the value for total wattage in Watt
        private byte[] _page25DataValues; // array that holds values datatypes from data page 25
        private byte[] _page16DataValues; // array that holds values datatypes from data page 16

        //variables for page 16
        public ushort _speed { get; set; } //speed in m/s

        public DeviceType DeviceType { get; } = DeviceType.Bike;

        public event OnDataReceive DataReceived;
        private Thread _thread;
        private bool _runningEmulator = false;

        /// <summary>
        /// Sets targetRPM, instantWattage and speed value and start running the emulator
        /// </summary>
        /// <param _panelUUID="targetRpm">Set the targeted RPM</param>
        /// <param _panelUUID="instantWattage">Set the instantaneous wattage send by the emulator</param>
        /// <param _panelUUID="speed">Set the speed send by the emulator</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public BicycleEmulator(byte targetRpm, ushort instantWattage, ushort speed)
        {
            this._targetRpm = targetRpm;
            if (instantWattage > 4095) throw new ArgumentOutOfRangeException(nameof(instantWattage));
            this._instantWattage = instantWattage;

            if (speed > 65.535) throw new ArgumentOutOfRangeException(nameof(speed));
            this._speed = ((ushort)(speed * 1000));

            _page16DataValues = InitializeDataValues();
            _page25DataValues = InitializeDataValues();
        }

        /// <summary>
        /// Initializes values for in the page arrays
        /// </summary>
        /// <returns></returns>
        private byte[] InitializeDataValues()
        {
            //these bytes change context depending on datapage 
            byte firstByte = 0x00;
            byte secondByte = 0x00;
            byte thirdByte = 0x00;
            byte fourthByte = 0x00;
            byte fifthByte = 0x00;
            byte sixthByte = 0x00;
            byte seventhByte = 0x00;
            byte eightByte = 0b00000000;

            byte[] dataValues = new byte[13]
                   {
                    _SYNCByte,
                    _lenghtByte,
                    _dataTypeByte,
                    _channelByte,
                    firstByte,
                    secondByte,
                    thirdByte,
                    fourthByte,
                    fifthByte,
                    sixthByte,
                    seventhByte,
                    eightByte,
                    0
                     };
            return dataValues;
        }

        /// <summary>
        /// Start sending data to emulate a connection being established
        /// </summary>
        public void Connect()
        {
            _runningEmulator = true;
            _thread = new Thread(RunEmulator);
            _thread.Start();
        }

        /// <summary>
        /// stops running the emulator SHEESH
        /// </summary>
        public void Disconnect()
        {
            _runningEmulator = false;
            _thread.Join();
        }

        /// <summary>
        /// Runs the emulator and updates values
        /// </summary>
        private void RunEmulator()
        {
            while (_runningEmulator)
            {
                UpdatePage16Values();
                UpdatePage25Values();
                Read(GetDataValues());
                Thread.Sleep(500);
            }

        }

        /// <summary>
        /// Updates the values for data page 16
        /// </summary>
        private void UpdatePage16Values()
        {
            byte[] oldValues = _page16DataValues;
            byte[] speedValues = Encoder.GetSpeedBytes((short)_speed);
            byte pageByte = 0x10;   //holds current page number
            byte equipmentTypeByte = 0x19; //holds value of equipment type; Refer table 8-8 
            byte timeByte = Encoder.IncreaseCounter(oldValues[6]); // byte for time
            byte distanceByte = Encoder.CalculateDistance((short)_speed, oldValues[7]); //byte for distance
            byte speedLSBByte = speedValues[0]; //least significant byte for instant speed
            byte speedMSBByte = speedValues[1]; //most significant byte for instant speed
            byte heartrateByte = 0xFF; //byte for heartrate
            byte finalMsgByte = 0b00110000; // Refer documentation at Table 8-9. Capabilities Bit Field and Table 8-10. FE State Bit Field to change correct bits


            _page16DataValues = new byte[13]
               {
                    _SYNCByte,
                    _lenghtByte,
                    _dataTypeByte,
                    _channelByte,
                    pageByte,
                    equipmentTypeByte,
                    timeByte,
                    distanceByte,
                    speedLSBByte,
                    speedMSBByte,
                    heartrateByte,
                    finalMsgByte,
                    0
               };
            _page16DataValues[12] = Encoder.GetChecksum(_page16DataValues); //gets checksum and puts it in the array

        }

        /// <summary>
        /// Updates the values for data page 25
        /// </summary>
        private void UpdatePage25Values()
        {
            byte[] oldValues = _page25DataValues;
            Byte[] wattageBytes = Encoder.GetWattages(_accumulatedWattage += _instantWattage, _instantWattage);

            byte pageByte = 0x19; // holds current page number
            byte countByte = Encoder.IncreaseCounter(oldValues[5]); // keeps track of how many byte arrays have been send
            byte candenceByte = GoToTargetRpm(oldValues[6]); //byte for RPM
            byte accumulatedPwrLSBByte = wattageBytes[0]; //least significant byte for accumulated power/wattage
            byte accumulatedPwrMSBByte = wattageBytes[1]; //most significant byte for accumulated power/wattage
            byte instantPwrLSBByte = wattageBytes[2]; //least significant byte for instant power/wattage
            byte instantPwrMSBByte = wattageBytes[3]; //most significant byte for instant power/wattage
            byte finalMsgByte = 0b00100000; // Refer documentation at Table 8-28. Flags Bit Field Description and Table 8-10. FE State Bit Field to change correct bits


            _page25DataValues = new byte[13]
               {
                    _SYNCByte,
                    _lenghtByte,
                    _dataTypeByte,
                    _channelByte,
                    pageByte,
                    countByte,
                    candenceByte,
                    accumulatedPwrLSBByte,
                    accumulatedPwrMSBByte,
                    instantPwrLSBByte,
                    instantPwrMSBByte,
                    finalMsgByte,
                    0
               };
            _page25DataValues[12] = Encoder.GetChecksum(_page25DataValues); //gets checksum and puts it in the array

        }

        /// <summary>
        /// gets correct data values based on page number
        /// </summary>
        /// <returns></returns>
        private byte[] GetDataValues()
        {
            byte pageNumber = Encoder.GetPage();
            switch (pageNumber)
            {
                case 0x10:
                    return _page16DataValues;
                case 0x19:
                    return _page25DataValues;
                case 0x50:
                    return new byte[13] { _SYNCByte, _lenghtByte, _dataTypeByte, _channelByte, 0x50, 0xff, 0xff, 0x10, 0x01, 0x23, 0xCA, 0xff, 0xB1 }; //standard array for page 80
                case 0x51:
                    return new byte[13] { _SYNCByte, _lenghtByte, _dataTypeByte, _channelByte, 0x51, 0xff, 0xff, 0x10, 0x01, 0x23, 0xCA, 0xff, 0xB0 }; //standard array for page 81
                default:
                    return _page16DataValues;

            }
        }

        /// <summary>
        /// Updates current RPM so it reaches target RPM
        /// </summary>
        /// <param _panelUUID="candenceByte"></param>
        /// <returns></returns>
        private byte GoToTargetRpm(byte candenceByte)
        {
            if (candenceByte < _targetRpm) candenceByte += 1;
            if (candenceByte > _instantWattage) candenceByte -= 1;
            return candenceByte;
        }

        /// <summary>
        /// Invokes the data array into the DataRecieved event
        /// </summary>
        /// <param _panelUUID="data"></param>
        public void Read(byte[] data)
        {
            DataReceived(data, DeviceType);
        }

        /// <summary>
        /// No data can be send to the emulator, so this method has no further implementation
        /// </summary>
        /// <param _panelUUID="sent"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Write(byte[] sent)
        {
            throw new NotImplementedException();
        }
    }
}