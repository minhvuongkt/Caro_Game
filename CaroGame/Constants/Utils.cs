using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Constants
{
    public class Utils
    {

        public static void WriteLog(string log)
        {
            File.AppendAllText("log.txt", log);
        }
        public static bool _checkEmpty(object obj)
        {
            return obj == null || string.IsNullOrEmpty(obj.ToString());
        }
        public static int RandomNumber(int max)
        {
            var random = new MyRandom();
            return random.NextInt(max);
        }
        public static int RandomNumber(int min, int max)
        {
            var random = new MyRandom();
            if (min <= max) return random.NextInt(min, max);
            (min, max) = (max, min);
            return random.NextInt(min, max);
        }
        public static double RandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public static string GetDiskSerialNumber()
        {
            string diskSerialNumber = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia");

            foreach (ManagementObject obj in searcher.Get())
            {
                diskSerialNumber = obj["SerialNumber"].ToString();
            }

            return diskSerialNumber.Trim();
        }
        public static string GetMotherboardId()
        {
            string motherboardId = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");

            foreach (ManagementObject obj in searcher.Get())
            {
                motherboardId = obj["SerialNumber"].ToString();
            }

            return motherboardId.Trim();
        }
        
    }
}
