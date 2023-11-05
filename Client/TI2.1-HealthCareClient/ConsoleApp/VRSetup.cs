using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI2._1_HealthCareClient.ConsoleApp
{
    public class VRSetup
    {
        private bool _isFirstTimeSetup;
        private bool _useSimpleScene;

        public VRSetup()
        {
        }

        public void Run()
        {
            Console.WriteLine("---Welkom bij de setup van de Remote Healthcare applicatie!---");
            Console.WriteLine("Is dit de eerste keer dat u de applicatie opstart op deze machine? Y/N:");

            bool firstUseSetup = false;
            while (!firstUseSetup)
            {
                var inputFirstUseSetup = Console.ReadLine();
                if (inputFirstUseSetup.Equals("Y"))
                {
                    _isFirstTimeSetup = true;
                    firstUseSetup = true;
                }
                else if (inputFirstUseSetup.Equals("N"))
                {
                    _isFirstTimeSetup = false;
                    firstUseSetup = true;
                }
                else
                {
                    Console.WriteLine("Voer een 'Y' (ja) in, of een 'N' (nee) in");
                }
            }


            Console.WriteLine(
                "Wilt u een een versimpelde VR scene (deze laadt sneller en kost minder resources om te runnen? Y/N");

            bool simpledVrSetup = false;
            while (!simpledVrSetup)
            {
                var inputSimpleSetup = Console.ReadLine();
                if (inputSimpleSetup.Equals("Y"))
                {
                    _useSimpleScene = true;
                    simpledVrSetup = true;
                }
                else if (inputSimpleSetup.Equals("N"))
                {
                    _useSimpleScene = false;
                    simpledVrSetup = true;
                }
                else
                {
                    Console.WriteLine("Voer een 'Y' (ja) in, of een 'N' (nee) in");
                }
            }

            Program.IsFirstTimeSetup = _isFirstTimeSetup;
            Program.UseSimpleScene = _useSimpleScene;
        }
    }
}