using Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BackendRunner
    {
        private static bool isRunning = false;

        public static void RunBackend()
        {
            if (!isRunning)
            {
                Program.Main(new string[] { });
            }
            isRunning = true;
        }
    }
}
