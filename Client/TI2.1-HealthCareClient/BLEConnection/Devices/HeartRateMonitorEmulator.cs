using System;
using System.Threading;
using TI2._1_HealthCareClient.BLEConnection.Translator;

namespace TI2._1_HealthCareClient.BLEConnection.Devices
{
    /// <summary>
    /// Class that emulates a heartrate monitor
    /// </summary>
    public class HeartRateMonitorEmulator : IBLEDevice
    {
        // Base values for first byte (last 3 bits are 0)
        private const bool _valueFormat = false; //false = UINT8, true = UINT16
        private const bool _skinContactSupport = true; //false = not supported, true = supported
        private const bool _skinContact = true; //false = not in contact with skin, true = in contact with skin
        private const bool _energyExpendedStatus = false; //false = energy expended data disabled, true = energy expended data enabled
        private const bool _rrIntervalValues = true; //makes use of rr interval status

        public byte AvgHeartbeat { get; set; }
        public int FluctuationFactor { get; set; }

        public DeviceType DeviceType { get; } = DeviceType.HeartrateSensor;

        public event OnDataReceive DataReceived;
        private Thread _thread;
        private bool _runningEmulator = false;

        /// <summary>
        /// Sets AvgHeartbeat and fluction of the average heartbeat
        /// </summary>
        /// <param _panelUUID="avgHeartbeat">The average heartbeat send by the emulator</param>
        /// <param _panelUUID="fluctuationFactor">The fluctuation in the average heartbeat</param>
        public HeartRateMonitorEmulator(byte avgHeartbeat, int fluctuationFactor)
        {
            this.AvgHeartbeat = avgHeartbeat;
            this.FluctuationFactor = fluctuationFactor;
        }

        /// <summary>
        /// Start sending data to emulate a connection being established
        /// </summary>
        public void Connect()
        {
            _runningEmulator = true;
            _thread = new Thread(RunEmulator);
            _thread.Start(); ;
        }

        /// <summary>
        /// Runs the emulator in a thread
        /// </summary>
        private void RunEmulator()
        {
            while (_runningEmulator)
            {
                Read(GetDataValues());
                Thread.Sleep(25);
            }
        }

        /// <summary>
        /// Updates values and puts the values of data in an array
        /// </summary>
        /// <returns>Array with data</returns>
        public byte[] GetDataValues()
        {
            byte[] dataValues = new byte[4];
            dataValues[0] = Encoder.EncodeFirstByte(new bool[] { false, false, false, _rrIntervalValues, _energyExpendedStatus, _skinContact, _skinContactSupport, _valueFormat });//3 false for unused bits in byte);
            dataValues[1] = GeneratateFluctuation();
            dataValues[2] = dataValues[3] = 0x00;
            return dataValues;
        }

        /// <summary>
        /// Generates a fluctuation for the heartbeat based on a fluctuation factor
        /// </summary>
        /// <returns>A heartbeat with fluctuation</returns>
        private byte GeneratateFluctuation()
        {
            Random randomizer = new Random();
            AvgHeartbeat += (byte)(randomizer.Next(0, FluctuationFactor + 1) - randomizer.Next(0, FluctuationFactor + 1));
            return AvgHeartbeat;

        }

        /// <summary>
        /// Stops running the emulator to emulate the connection being broken
        /// </summary>
        public void Disconnect()
        {
            _runningEmulator = false;
            _thread.Join();

        }

        /// <summary>
        /// Invokes the data array into the DataRecieved event
        /// </summary>
        /// <param _panelUUID="data"></param>
        public void Read(byte[] data)
        {
            DataReceived?.Invoke(data, DeviceType);
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