using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareClient.VRConnection.Commands.Node.DataAttributes
{
    public class SceneNodeUpdateTransformDataAttribute
    {
        public int[] position { get; set; }
        public double scale { get; set; }
        public int[] rotation { get; set; }
    }
}