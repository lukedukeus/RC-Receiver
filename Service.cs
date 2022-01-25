using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wireless_RC_Controller_RX
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Console.WriteLine("Starting");
            USBListner listner = new USBListner();

            Helpers.getCOMPorts();
            // connect to arduino if it exists
            // emulate the game driver

        }

        protected override void OnStop()
        {
            Console.WriteLine("Stopping");

            // stop emulation
            // disconnect com port
        }


        internal void TestStartupAndStop()
        {
            this.OnStart(null);
            Thread.Sleep(Timeout.Infinite);
            this.OnStop();
        }


    }
}
