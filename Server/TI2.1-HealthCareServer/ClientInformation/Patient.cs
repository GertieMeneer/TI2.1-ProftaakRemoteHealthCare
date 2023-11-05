using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareServer.ClientInformation
{
    /// <summary>
    /// Class responsible for creating patient object
    /// </summary>
    public class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public bool SessionStarted { get; set; }

        /// <summary>
        /// Constructor for creating new patient
        /// </summary>
        /// <param name="patientId">patientid from the patient</param>
        /// <param name="name">patientname from the patient</param>
        public Patient(string patientId, string name)
        {
            PatientId = patientId;
            Name = name;
            SessionStarted = false;
        }
    }
}
