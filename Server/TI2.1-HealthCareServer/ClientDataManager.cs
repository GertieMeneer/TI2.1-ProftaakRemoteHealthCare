using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TI2._1_HealthCareServer.ClientInformation;

namespace TI2._1_HealthCareServer
{
    /// <summary>
    /// This class is responsible for managing all the csv files with patient data
    /// </summary>
    public class ClientDataManager
    {
        private string _fileName;
        private bool _run = true;
        private bool _initialize = true;
        private string _data = "Time, Heartrate, Speed, Distance";
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        private int _seconds = 0;
        private int _minutes = 0;


        /// <summary>
        /// Create a new file for a patient
        /// </summary>
        /// <param name="patientId">The patient</param>
        public void CreateFile(string patientId)
        {
            Console.WriteLine("creating file");
            int sessionid = 1;
            DateTime current = DateTime.Now;
            string currentdate = current.ToString("dd-MM-yyyy");

            //Check if a file already exists
            while (File.Exists($"data_{currentdate}_{patientId}_{sessionid}.csv"))
            {
                sessionid++;
            }

            _fileName = $"data_{currentdate}_{patientId}_{sessionid}.csv";

            _fileStream = new FileStream(_fileName, FileMode.Append, FileAccess.Write);
            _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8);

            Thread writerThread = new Thread(() => Start());
            writerThread.Start();
        }


        /// <summary>
        /// This function always runs on a new Thread.
        /// It checks if new data has been generated and prints it to the file.
        /// </summary>
        private void Start()
        {
            Console.WriteLine("starting writer thread");
            try
            {
                Thread timerThread = new Thread(() => StartTimer());
                timerThread.Start();

                string olddata = "";

                while (_run)
                {
                    // Handle data updates triggered by the event
                    if (_data != olddata)
                    {
                        if (_initialize)
                        {
                            _streamWriter.WriteLine($"{_data}");
                            _initialize = false;
                        }
                        else
                        {
                            _streamWriter.WriteLine($"{_minutes}:{_seconds}, {_data}");
                        }

                        _streamWriter.Flush(); //zodat data gelijk geschreven wordt (niet in de buffer blijft)
                        olddata = _data;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing data to file: {ex.Message}");
            }
            finally
            {
                _streamWriter.Close();
                _fileStream.Close();
            }
        }

        /// <summary>
        /// This function encrypts data using the Aes algorithm
        /// </summary>
        /// <param name="data">Unencrypted data</param>
        /// <returns>Encrypted data</returns>
        private string EncryptData(string data)
        {
            using (Aes aesAlg = Aes.Create())
            {
                string password = "super secret password"; //Password to encrypt data
                aesAlg.Key =
                    new Rfc2898DeriveBytes(password, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 },
                        10000).GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = new byte[16];

                //encrypt the data
                byte[] encryptedBytes;
                using (MemoryStream outputStream = new MemoryStream())
                using (CryptoStream encryptor =
                       new CryptoStream(outputStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter streamWriter = new StreamWriter(encryptor))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    encryptor.FlushFinalBlock();
                    encryptedBytes = outputStream.ToArray();
                }

                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public string DecryptCsvFile(string filePath, string password)
        {
            string decryptedData = "";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key =
                    new Rfc2898DeriveBytes(password, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 },
                        10000).GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = new byte[16];

                using (FileStream inputFile = new FileStream(filePath, FileMode.Open))
                using (CryptoStream decryptor =
                       new CryptoStream(inputFile, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (MemoryStream outputStream = new MemoryStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = decryptor.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }

                    decryptedData = Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }

            return decryptedData;
        }


        /// <summary>
        /// Stops the data writer thread
        /// </summary>
        public void Stop()
        {
            _run = false;
        }

        /// <summary>
        /// Save new data so that data writer thread can write it
        /// </summary>
        /// <param name="newData">The new data</param>
        public void SaveData(string newData)
        {
            _data = newData;
            Console.WriteLine($"{_data}");
        }

        private void StartTimer()
        {
            while (_run)
            {
                if (_seconds == 59)
                {
                    _minutes++;
                    _seconds = 0;
                }
                else
                {
                    _seconds++;
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Get all historic sessions that are not active anymore
        /// </summary>
        /// <returns>List with all historic sessions</returns>
        public static List<HistoricSession> GetHistoricSessions()
        {
            List<HistoricSession> historicSessions = new List<HistoricSession>();

            string todayDate = DateTime.Now.ToString("dd-MM-yyyy");

            //Iterate through all files in the directory
            foreach (string filePath in Directory.GetFiles(Environment.CurrentDirectory))
            {
                string fileName = Path.GetFileName(filePath);
                //Split filename in parts so information can be extracted
                string[] parts = fileName.Split('_');
                if (parts.Length == 4)
                {
                    string patientId = parts[2];

                    //Check if the patient from the file has active session
                    bool isPatientActive = ClientInfoManager.GetPatientsWithActiveSessions()
                        .Any(patient => patient.PatientId == patientId);

                    if (isPatientActive)
                    {
                        string sessionDate = parts[1];

                        //Check if the sessionDate from file is not today: then it's still historic data
                        if (sessionDate != todayDate)
                        {
                            string patientName = ClientInfoManager.GetPatientNameById(patientId);
                            HistoricSession historicSession = new HistoricSession
                            {
                                PatientName = patientName,
                                SessionDate = sessionDate
                            };
                            historicSessions.Add(historicSession);
                        }
                    }
                    //If patient not active it's historic for sure
                    else
                    {
                        string sessionDate = parts[1];
                        string patientName = ClientInfoManager.GetPatientNameById(patientId);
                        HistoricSession historicSession = new HistoricSession
                        {
                            PatientName = patientName,
                            SessionDate = sessionDate
                        };
                        historicSessions.Add(historicSession);
                    }
                }
            }

            return historicSessions;
        }
    }
}