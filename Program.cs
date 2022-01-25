using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Wireless_RC_Controller_RX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            //if (Environment.UserInteractive)                                            // if a user is running it manually, show output, the pause
            //{
                new Service().TestStartupAndStop();
            //}
            //else
            //{
                ServiceBase[] ServicesToRun;                                            // else run as service
                ServicesToRun = new ServiceBase[]
                {
                new Service()
                };
                ServiceBase.Run(ServicesToRun);
            //}





        }
    }
}
