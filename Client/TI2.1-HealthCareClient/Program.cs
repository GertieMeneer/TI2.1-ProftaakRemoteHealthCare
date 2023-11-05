using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using TI2._1_HealthCareClient;
using TI2._1_HealthCareClient.ServerConnection;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend.DataAttributes;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend;
using TI2._1_HealthCareClient.VRConnection;
using TI2._1_HealthCareClient.BLEConnection.Translator;
using TI2._1_HealthCareClient.BLEConnection.Devices;
using TI2._1_HealthCareClient.BLEConnection;
using TI2._1_HealthCareClient.ConsoleApp;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient.DataAttributes;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectBike;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectPatient;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectPatient.DataAttributes;
using TI2._1_HealthCareClient.ServerConnection.Commands.SessionStart;
using TI2._1_HealthCareClient.ServerConnection.Commands.SessionStart.DataAttributes;

namespace TI2._1_HealthCareClient
{
    public class Program
    {
        private static PatientSetup _patientSetup = new PatientSetup();

        private static string _serverIp = "127.0.0.1";
        private static int _serverPort = 12345;
        public static ServerClientConnection ServerConnection = new ServerClientConnection(_serverIp, _serverPort);

        private static string _vrServerIp = "145.48.6.10";
        private static int _vrServerPort = 6666;
        private static string _deviceName = Environment.MachineName;
        private static bool _debugMode = true;

        public static bool IsFirstTimeSetup;
        public static bool UseSimpleScene;

        public static VrManager VrManager = new VrManager();


        public static Translator Translator;

        private static int _dataCount;
        private static List<FEData> _feDataList = new List<FEData>();

        public static AllDataAttribute Data;

        public static BicycleEmulator BicycleEmulator = new BicycleEmulator(60, 80, 20);
        public static HeartRateMonitorEmulator HeartRateMonitorEmulator = new HeartRateMonitorEmulator(100, 2);

        public static FitnessBLE fiets = new FitnessBLE("24517", DeviceType.Bike);



        public static void Main(string[] args)
        {
            //Connect with server
            Thread serverThread = new Thread(() => ServerConnection.Connect());
            serverThread.Start();

            //Connect with vr
            VrManager.DebugMode = _debugMode;

            //Setup translator
            Translator = new Translator();
            Translator.DataTranslated += OnDataTranslated;

            Translator.AddBlEDevice(fiets);
            

            //TE BLOEFOOF DEFIJS IS REDIE TO PER

            // var hartslagsensor = new FitnessBLE(DeviceType.HeartrateSensor);
            // Translator.AddBlEDevice(hartslagsensor);


            // Translator.AddBlEDevice(BicycleEmulator);
            Translator.AddBlEDevice(HeartRateMonitorEmulator);

            // Translator.AddBlEDevice(new FitnessBLE(DeviceType.HeartrateSensor));


            VrManager.NetworkEngineClosed += (sender, e) =>
            {
                ServerConnection.StopSendingFEData();
                _patientSetup.RemovePatient();

                VrManager.Disconnect();
                foreach (var device in Translator.Devices)
                {
                    device.Disconnect();
                }

                _patientSetup.SetupPatient();
            };

            VRSetup vrSetup = new VRSetup();
            vrSetup.Run();

            _patientSetup.SetupPatient();

            Thread.Sleep(1000);

            // hartslagsensor.Connect();
            //
            // Thread feDataThread = new Thread(() => { ServerConnection.SendFEData(); });
            // feDataThread.Start();
        }

        public static void SetupVR()
        {
            VrManager.OpenNetworkEngine("sim.bat");
            Thread.Sleep(1000);
            VrManager.Connect(_vrServerIp, _vrServerPort, _deviceName);
            Thread.Sleep(1000);
            VrManager.LoadScene("Farm", IsFirstTimeSetup, UseSimpleScene);
        }


        /// <summary>
        /// This method is used when the Transform is translated 
        /// </summary>
        /// <param name="feData">incoming Transform form the translator</param>
        private static void OnDataTranslated(FEData feData)
        {
            // Thread.Sleep(1);
            if (_dataCount < 10) // the number is the amount of Transform used to calculate the median 
            {
                _feDataList.Add(feData);
                _dataCount++;
                return;
            }

            _dataCount = 0;
            Data = CalculateMedian(_feDataList);

            _feDataList.Clear();

            Console.WriteLine(feData.Speed.ToString());


            if (Data.Speed != null)
            {
                Console.WriteLine("Setting speed :" + Data.Speed);
                VrManager.SetSpeed(Convert.ToDecimal(Data.Speed));
            }

            if (Data.HeartRate != null)
            {
                VrManager.SetHeartRate(Convert.ToInt32(Data.HeartRate));
            }
        }

        /// <summary>
        ///  Calculates the median of the given values
        /// </summary>
        /// <param name="values">The values to calculate the median from</param>
        /// <returns>AllDataAttribute with the median values</returns>
        private static AllDataAttribute CalculateMedian(List<FEData> values)
        {
            var distanceList = new List<double>();
            var heartRateList = new List<double>();
            var speedList = new List<double>();

            Thread.Sleep(100);
            foreach (var item in values)
            {
                if (!double.IsNaN(item.Distance))
                {
                    distanceList.Add(item.Distance);
                }

                if (item.HeartRate != 0)
                {
                    heartRateList.Add(item.HeartRate);
                }

                if (!double.IsNaN(item.Speed))
                {
                    speedList.Add(item.Speed);
                }
            }

            distanceList.Sort();
            heartRateList.Sort();
            speedList.Sort();

            var FEData = new AllDataAttribute();

            if (distanceList.Count > 0)
                FEData.Distance = distanceList[(int)(Math.Ceiling(distanceList.Count / 2.0) - 1)].ToString();
            if (heartRateList.Count > 0)
                FEData.HeartRate = heartRateList[(int)(Math.Ceiling(heartRateList.Count / 2.0) - 1)].ToString();
            if (speedList.Count > 0)
                FEData.Speed = speedList[(int)(Math.Ceiling(speedList.Count / 2.0) - 1)].ToString();

            return FEData;
        }
    }
}