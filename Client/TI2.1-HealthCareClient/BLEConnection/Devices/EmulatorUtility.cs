namespace TI2._1_HealthCareClient.BLEConnection.Devices
{
    public static class Encoder
    {
        /// <summary>
        /// converts the array of bytevalues to a byte
        /// </summary>
        /// <param _panelUUID="byteValues"></param>
        /// <returns></returns>
        public static byte EncodeFirstByte(bool[] byteValues)
        {
            byte firstByte = 0x00;

            foreach (bool value in byteValues)
            {
                firstByte = (byte)(firstByte << 1);
                if (value) firstByte = (byte)(firstByte | 0b1);

            }
            //Console.WriteLine(Convert.ToString(firstByte, 2).PadLeft(8, '0'));
            return firstByte;

        }

        /// <summary>
        /// gets checksum from all values in array
        /// </summary>
        /// <param _panelUUID="dataValues"></param>
        /// <returns></returns>
        public static byte GetChecksum(byte[] dataValues)
        {
            byte checksum = 0;
            foreach (byte data in dataValues)
            {
                checksum = (byte)(checksum ^ data);
            }
            return checksum;
        }

        /// <summary>
        /// converts accumulatedWattage and InstantWattage in an byte array 
        /// </summary>
        /// <param _panelUUID="accumulatedWattage"></param>
        /// <param _panelUUID="instantWattage"></param>
        /// <returns></returns>
        public static byte[] GetWattages(ushort accumulatedWattage, ushort instantWattage)
        {
            byte accumulatedPwrLSBByte = (byte)(accumulatedWattage & 0xFF);
            byte accumulatedPwrMSBByte = (byte)(accumulatedWattage >> 7);
            byte instantPwrLSBByte = (byte)(instantWattage & 0xFF);
            byte instantPwrMSBByte = (byte)(instantWattage >> 7);
            return new byte[] { accumulatedPwrLSBByte, accumulatedPwrMSBByte, instantPwrLSBByte, instantPwrMSBByte };
        }

        /// <summary>
        /// gets page number based on amount of messages send
        /// </summary>
        private static int _messageCounter = 0;
        public static byte GetPage()
        {
            _messageCounter++;
            // Console.WriteLine(_messagecounter.ToString());
            if (_messageCounter > 135) _messageCounter = 0;
            if (_messageCounter > 133) return 0x51;
            if (_messageCounter > 131) return 0x50;
            if (_messageCounter % 5 == 0) return 0x10;
            return 0x19;
        }

        public static byte IncreaseCounter(byte initialValue)
        {
            if (initialValue + 1 > byte.MaxValue) return 0;
            return (byte)(initialValue + 1);
        }
        public static byte CalculateDistance(short speed, byte time)
        {
            return (byte)((speed / 1000) * (time * 0.25));

        }

        public static byte[] GetSpeedBytes(short speed)
        {

            byte speedLSBByte = (byte)(speed & 0xFF);
            byte speedMSBByte = (byte)(speed >> 7);
            return new byte[] { speedLSBByte, speedMSBByte };
        }
    }
}
