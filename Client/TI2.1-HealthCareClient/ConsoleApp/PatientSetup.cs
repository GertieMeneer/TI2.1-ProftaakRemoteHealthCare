using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionConnectPatient.DataAttributes;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectPatient;
using TI2._1_HealthCareClient.ServerConnection.Commands.ConnectionDisconnectPatient.DataAttributes;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend;
using TI2._1_HealthCareClient.ServerConnection.Commands.DataSend.DataAttributes;
using TI2._1_HealthCareClient.ServerConnection.Commands.SessionStop;
using TI2._1_HealthCareClient.ServerConnection.Commands.SessionStop.DataAttributes;
using TI2._1_HealthCareClient.VRConnection;

namespace TI2._1_HealthCareClient.ConsoleApp
{
    public class PatientSetup
    {
        public static string PatientId;
        public static string PatientName;
        public PatientSetup()
        {

        }

        public void SetupPatient()
        {
            Console.WriteLine("Welkom patiënt! Scan uw pas om uzelf te identificeren (voer patiënt-id in)");
            bool patientIdSetup = false;

            while (!patientIdSetup)
            {
                PatientId = Console.ReadLine();
                Console.WriteLine($"Uw patiënt-id is: {PatientId}, klopt dit? Y/N");
                var patientIdSetupVar = Console.ReadLine();
                if (patientIdSetupVar.Equals("Y"))
                {
                    patientIdSetup = true;
                }
                else if (patientIdSetupVar.Equals("N"))
                {
                    Console.WriteLine("Scan uw pas opnieuw (voer uw patiënt-id opnieuw in");
                }
                else
                {
                    Console.WriteLine("Voer een 'Y' (ja) in, of een 'N' (nee) in");
                }
            }

            Console.WriteLine("Bedankt voor het scannen! Wat is uw naam?");
            PatientName = Console.ReadLine();

            Console.WriteLine(
                $"Welkom {PatientName}, zet gelieve de VR headset op. Uw dokter zal uw inspanningstest in een ogenblik starten.");

            Program.ServerConnection.Write(new ConnectionConnectPatient()
                { Data = new TypeDataAttribute() { Patientid = PatientId, Patientname = PatientName } });

            Program.SetupVR();
        }

        public void RemovePatient()
        {
            Program.ServerConnection.Write(new SessionStop() {data = new SessionStopDataAttributes() {patientid = PatientId}});

            Program.ServerConnection.Write(new ConnectionDisconnectPatient()
                { data = new InfoDataAttribute() { patientid = PatientId } });
        }
    }
}
