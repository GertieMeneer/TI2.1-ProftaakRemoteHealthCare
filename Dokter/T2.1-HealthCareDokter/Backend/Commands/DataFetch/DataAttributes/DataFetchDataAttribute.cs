using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T2._1_HealthCareDokter.Backend.Commands.DataFetch.DataAttributes
{
    public class DataFetchDataAttribute
    {
        public string PatientId { get; set; }
        public string SessionId { get; set; }
        public DateTime Date { get; set; }
    }
}
