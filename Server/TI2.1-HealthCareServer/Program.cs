using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TI2._1_HealthCareServer
{
    public class Program
    {
        /// <summary>
        /// Runs the server
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // testDencryptCSV();
            Server server = new Server();
            server.Start();
        }

        private static void testDencryptCSV()
        {
            ClientDataManager cdm = new ClientDataManager();
            string data = cdm.DecryptCsvFile("data_30-10-2023_345_1.csv", "super secret password");
            Console.WriteLine(data);
        }

        /// <summary>
        /// For creating the encrypted doktercridentials file
        /// </summary>
        private static void TestDokterCridentials()
        {
            // MakeFile(); //makes a cridentials file
            ReadFile(); //reads the file, encrypts and shows values
        }

        /// <summary>
        /// Makes the encrypted file
        /// </summary>
        private static void MakeFile()
        {
            // Generate a secure key from the password using a key derivation function
            var salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            var keyDerivation =
                new Rfc2898DeriveBytes("super secret password", salt,
                    10000); // You can adjust the iteration count as needed

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyDerivation.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = new byte[16]; // Initialize Vector (IV) to a fixed value or generate a random IV.

                // Create the JSON data
                var jsonToEncrypt = new JObject
                {
                    ["password"] = "boktor",
                    ["username"] = "doktor"
                };
                string jsonData = jsonToEncrypt.ToString();

                using (MemoryStream outputStream = new MemoryStream())
                using (CryptoStream encryptor =
                       new CryptoStream(outputStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter streamWriter = new StreamWriter(encryptor))
                {
                    streamWriter.Write(jsonData);
                    streamWriter.Flush();
                    encryptor.FlushFinalBlock();
                    File.WriteAllBytes("doktercridentials", outputStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Test for reading the encrypted file
        /// </summary>
        /// <returns>Data inside the encrypted file</returns>
        private static JObject ReadFile()
        {
            //Create Aes object
            using (Aes aesAlg = Aes.Create())
            {
                //Get Aes key from password
                aesAlg.Key = new Rfc2898DeriveBytes("super secret password",
                    new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, 10000).GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = new byte[16];

                using (FileStream inputFile = new FileStream("doktercridentials", FileMode.Open))
                using (MemoryStream outputStream = new MemoryStream())
                using (CryptoStream decryptor =
                       new CryptoStream(outputStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    //Decrypt data and return it
                    inputFile.CopyTo(decryptor);
                    decryptor.FlushFinalBlock();
                    byte[] decryptedBytes = outputStream.ToArray();
                    string decryptedJson = Encoding.UTF8.GetString(decryptedBytes);
                    return JObject.Parse(decryptedJson);
                }
            }
        }
    }
}