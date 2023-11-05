using System;

namespace TI2._1_HealthCareClient.BLEConnection.Translator
{

    public class BicycleTranslator : ITranslatorStrategy
    {
        // General FE Transform
        private double _calcTimer;
        private int _timerResetCount, _calcDistance, _distanceResetCounter;

        // Specific Trainer/Stationary Bike Transform
        private int _calcPower, _powerResetCounter;

        /// <summary>
        ///  Translates the data from the BLE device to a FEData object
        /// </summary>
        /// <param _panelUUID="data"></param>
        /// <returns>FEData</returns>
        /// <exception cref="Exception"></exception>
        public FEData Translate(byte[] data)
        {
            var feData = new FEData();

            // checksum checker
            byte checkSum = 0x00;
            for (var i = 0; i < data.Length - 1; i++)
            {
                checkSum ^= data[i];
            }

            if (checkSum != data[data.Length - 1])
            {
                throw new Exception(
                    "Checksum failed, try making some better code nerd >:("); // exception for when the checksum fails
            }

            var page = (DataPages)data[4]; // Byte 4 is the page number of the bike`

            feData.TimeStamp = new TimeStampMaker().GetCurrentTimeStamp();
            feData.DataType = "Bicycle";
            feData.DataPage = data[4];

            switch (page)
            {
                case DataPages.GeneralFeData:
                    if (data[6] < _calcTimer)
                    {
                        // Byte 6 is the timer upto 255
                        _timerResetCount++; // counts the amount of times the timer has reset
                    }

                    _calcTimer = data[6];

                    if (data[7] < _calcDistance)
                    {
                        // Byte 7 is the distance upto 255
                        _distanceResetCounter++; // counts the amount of times the distance has reset
                    }

                    _calcDistance = data[7];


                    feData.Timer = (_timerResetCount * 256 + _calcTimer) / 4; // calculates the whole timer
                    feData.Distance = (_distanceResetCounter * 256) + _calcDistance; // calculates the whole distance
                    feData.Speed = (((data[9] << 8) | data[8]) / 1000.0) * 3.6; // Byte 8 and 9 are the speed

                    break;

                case DataPages.SpecificTrainerOrStationaryBikeData:

                    var accumulatedPower = (data[8] << 8) | data[7]; // Byte 7 and 8 are the accumulated power
                    if (accumulatedPower < _calcPower)
                    {
                        _powerResetCounter++; // counts the amount of times the accumulated power has reset
                    }

                    _calcPower = accumulatedPower;
                    feData.Cadence = data[6]; // Byte 6 is the cadence
                    feData.TotalPower = _powerResetCounter * 65536 + _calcPower; // calculates the whole accumulated power
                    feData.CurrentPower = (data[10] & 0x0F) << 8 | data[9]; // Byte 9 and 10 are the current power | todo: check if this is correct

                    break;
                default:
                    Console.WriteLine("not an valid byte or just not implemented yet | Byte: {0:X}", data[4]);
                    break;
            }

            return feData; // returns the FEData object
        }
    }
}
