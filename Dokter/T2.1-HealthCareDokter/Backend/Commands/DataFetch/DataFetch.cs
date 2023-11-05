using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.DataFetch.DataAttributes;

namespace T2._1_HealthCareDokter.Backend.Commands.DataFetch
{
    public class DataFetch : IDokterCommand
    {
        public string Command = "data/fetch";
        public DataFetchDataAttribute Data { get; set; }
    }
}
