using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.BikeSetResistance.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.BikeSetResistance
{
    public class BikeSetResistance : IDokterCommand
    {
        public string Command = "bike/setresistance";
        public BikeSetResistanceDataAttribute Data { get; set; }
    }
}
