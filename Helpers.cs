using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO.Ports;
using vJoyInterfaceWrap;

namespace Wireless_RC_Controller_RX
{
    class Helpers
    {
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;

        public static long xmax;
        public static long ymax;
        public static long xmin;
        public static long ymin;
        public static long rxmax;
        public static long rymax;
        public static long rxmin;
        public static long rymin;

        public static List<String> ConnectedCOMs = new List<string>();

        public static void getCOMPorts()
        {
            List<SerialPortInfo> ports = null;
            string qry = "SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM_)%'";
            //or:
            //qry = "SELECT * FROM Win32_SerialPort";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", qry))
            {
                ports = searcher.Get().Cast<ManagementBaseObject>()
                    .Select(sp => new SerialPortInfo((string)sp["DeviceID"], (string)sp["Caption"]))
                    .ToList();
            }
            foreach (SerialPortInfo mp in ports)
            {
                int start = mp.Name.IndexOf("(") + 1;
                int end = mp.Name.IndexOf(")", start);
                string comPort = mp.Name.Substring(start, end - start);

                connectReciever(comPort);
            }
        }

        public static int map(long input, long outMin, long outMax)
        {
            int inMin = 1000;
            int inMax = 2000;

            return (int)(outMin + ((outMax - outMin) / (inMax - inMin)) * (input - inMin));

        }

        public static void connectReciever(string comPort)
        {
            if (ConnectedCOMs.Contains(comPort))
            {
                return;
            }

            Console.WriteLine("Connecting to {0}", comPort);

            SerialPort port = new SerialPort();

            port = new SerialPort();
            port.BaudRate = 115200;
            port.PortName = comPort;

            port.Open();
            ConnectedCOMs.Add(comPort);

            bool justOpened = true;
            bool controllerExists = false;



            while (port.IsOpen)
            {

                    string data = port.ReadLine();
                    if (!data.Contains("A6N") && justOpened == false)
                    {
                        port.Close();
                    }


                    if (controllerExists == false)
                    {
                        joystick = new vJoy();
                        controllerExists = true;
                        joystick.ResetVJD(id);

                        VjdStat status = joystick.GetVJDStatus(id);
                        switch (status)
                        {
                            case VjdStat.VJD_STAT_OWN:
                                Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                                break;
                            case VjdStat.VJD_STAT_FREE:
                                Console.WriteLine("vJoy Device {0} is free\n", id);
                                break;
                            case VjdStat.VJD_STAT_BUSY:
                                Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                                return;
                            case VjdStat.VJD_STAT_MISS:
                                Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                                return;
                            default:
                                Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                                return;
                        };

                        if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
                        {
                            Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                        }
                        else
                            Console.WriteLine("Acquired: vJoy device number {0}.\n", id);

                        joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref xmax);
                        joystick.GetVJDAxisMin(id, HID_USAGES.HID_USAGE_X, ref xmin);
                        joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_Y, ref ymax);
                        joystick.GetVJDAxisMin(id, HID_USAGES.HID_USAGE_Y, ref ymin);
                        joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_RX, ref rxmax);
                        joystick.GetVJDAxisMin(id, HID_USAGES.HID_USAGE_RX, ref rxmin);
                        joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_RY, ref rymax);
                        joystick.GetVJDAxisMin(id, HID_USAGES.HID_USAGE_RY, ref rymin);


                    }
                    else
                    {



                        string[] values = data.Split(' ');

                        int ch1 = 1500;
                        int ch2 = 1500;
                        int ch3 = 1500;
                        int ch4 = 1500;
                        int sw1 = 1500;
                        int sw2 = 1500;

                    if (values.Length == 7)
                    {
                        int.TryParse(values[1], out ch1);
                        int.TryParse(values[2], out ch2);
                        int.TryParse(values[3], out ch3);
                        int.TryParse(values[4], out ch4);

                        int.TryParse(values[5], out sw1);
                        int.TryParse(values[6], out sw2);

                        joystick.SetAxis(map(ch1, xmin, xmax), id, HID_USAGES.HID_USAGE_X);
                        joystick.SetAxis(map(ch2, rxmin, rxmax), id, HID_USAGES.HID_USAGE_RX);
                        joystick.SetAxis(map(ch3, ymin, ymax), id, HID_USAGES.HID_USAGE_Y);
                        joystick.SetAxis(map(ch4, rymin, rymax), id, HID_USAGES.HID_USAGE_RY);


                        joystick.SetBtn(sw1 > 1500, id, 1);
                        joystick.SetBtn(sw2 > 1500, id, 4);
                    }




                    }



                    justOpened = false;
  

            }

            ConnectedCOMs.Remove(comPort);

            connectReciever(comPort);

        }

    }

   class USBListner
   {
        public USBListner()
        {

            var watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent 1");

            watcher.EventArrived += new EventArrivedEventHandler(usbChanged);
            watcher.Query = query;
            watcher.Start();

        }

        private void usbChanged(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("USB ADD or REMOVE");
            Helpers.getCOMPorts();
        }



    }


    public class SerialPortInfo
    {
        public SerialPortInfo(string _id, string _name)
        {
            ID = _id;
            Name = _name;
        }

        public string ID = "";
        public string Name = "";

    }



}
