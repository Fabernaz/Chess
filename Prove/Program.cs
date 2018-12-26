using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Prove
{
    class Program
    {
        static InfoAsker<string> asker;

        static void Main(string[] args)
        {
            asker = new InfoAsker<string>();
            asker.AskingInfoEvent += asker_AskingInfoEvent;
            asker.AskAndWaitForInfo();
        }

        private static void asker_AskingInfoEvent(object sender, EventArgs e)
        {
            Console.WriteLine("Inserisci");
            asker.ProvideRequestedInfo(Console.ReadLine());
        }
    }
}
