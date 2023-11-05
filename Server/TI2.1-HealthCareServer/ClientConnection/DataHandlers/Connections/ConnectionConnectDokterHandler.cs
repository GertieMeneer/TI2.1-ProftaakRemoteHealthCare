using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TI2._1_HealthCareServer.ClientConnection.Commands.StatusMessage;
using TI2._1_HealthCareServer.ClientInformation;

namespace TI2._1_HealthCareServer.ClientConnection.DataHandlers.Connections
{
    public class ConnectionConnectDokterHandler
    {
        private JObject _jObject;
        private SslStream _sslStream;

        public ConnectionConnectDokterHandler(JObject jObject, SslStream sslStream)
        {
            _jObject = jObject;
            _sslStream = sslStream;
        }

        public void Handle()
        {
            var command = _jObject["command"].ToString();

            var password = _jObject["data"]["password"].ToString();
            var username = _jObject["data"]["username"].ToString();

            var storedPassword = DecryptCridentials()["password"].ToString();
            var storedUsername = DecryptCridentials()["username"].ToString();

            if (password == storedPassword && username == storedUsername)
            {
                Dokter.AddDokter(_sslStream);
                Communicator.WriteToDokter(new StatusMessage() { Command = command, Data = new StatusOk() });
            }
            else
            {
                Dokter.AddDokter(_sslStream);
                Communicator.WriteToDokter(new StatusMessage()
                {
                    Command = command,
                    Data = new StatusError()
                    {
                        Error = "Username and/or password incorrect, or a docter is already logged in."
                    }
                });
            }
        }

        private JObject DecryptCridentials()
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = new Rfc2898DeriveBytes("super secret password", new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, 10000).GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = new byte[16];

                using (FileStream inputFile = new FileStream("doktercridentials", FileMode.Open))
                using (MemoryStream outputStream = new MemoryStream())
                using (CryptoStream decryptor = new CryptoStream(outputStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                {
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