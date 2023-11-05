using System;

namespace TI2._1_HealthCareClient.BLEConnection.Translator
{
    public class HeartRateMonitorTranslator : ITranslatorStrategy
    {
        /// <summary>
        /// Translates the data from the BLE device to a FEData object
        /// </summary>
        /// <param _panelUUID="data"></param>
        /// <returns>FEData</returns>
        public FEData Translate(byte[] data)
        {
            var feData = new FEData
            {
                TimeStamp = new TimeStampMaker().GetCurrentTimeStamp(),
                DataType = "HeartMonitor",
                HeartRate = data[1] // Byte 1 is the heart rate
            };
            return feData; // returns the FEData object
        }
    }

    internal class TimeStampMaker
    {
        /// <summary>
        /// This method is used to get the current time stamp
        /// </summary>
        /// <returns>string</returns>
        internal string GetCurrentTimeStamp()
        {
            // gets the current time in the local time zone
            var timeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local); 
            // returns the current time stamp in the format of yyyy-MM-dd HH:mm:ss.fff
            return $"{timeZone.Year:0000}-{timeZone.Month:00}-{timeZone.Day:00} {timeZone.Hour:00}:{timeZone.Minute:00}:{timeZone.Second:00}.{timeZone.Millisecond:000}";
        }
    }
}
