using System;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections;

namespace System.EnvironmentInfo
{
    /// <summary>
    /// Descripción breve de ComputerInfo.
    /// </summary>
    public class ComputerInfo
    {
        /// <summary>
        /// Número de serie del disco duro donde esta instalado el sistema operativo
        /// </summary>
        /// <param name="strDriveLetter"></param>
        /// <returns></returns> 
        public static string GetVolumeSerialSystem()
        {
            string strDriveLetter = Environment.SystemDirectory.Substring(0, 1);
            ManagementObject disk =
                new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            disk.Get();
            return disk["VolumeSerialNumber"].ToString();
        }
        /// <summary>
        /// Número de serie del disco duro
        /// </summary>
        /// <param name="strDriveLetter"></param>
        /// <returns></returns> 
        public static string GetVolumeSerial(string strDriveLetter)
        {
            if (strDriveLetter == "" || strDriveLetter == null) strDriveLetter = "C";
            ManagementObject disk =
                new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            disk.Get();
            return disk["VolumeSerialNumber"].ToString();
        }
        /// <summary>
        /// get operating system user
        /// </summary>
        /// <returns></returns>
        public static string GetOperatingSystem()
        {
            // PlatformID	Major Version	Minor Version	Operating System  
            // -------------------------------------------------------------
            // Win32Windows	>= 4			0				Win95  
            // Win32Windows >= 4			> 0 && < 90		Win98  
            // Win32Windows >= 4			> 0 && >= 90	WinMe 
            // Win32NT		<= 4			0				WinNT  
            // Win32NT 5	5				0				Win2K  
            // Win32NT 6	6   			> 0				Vista

            string ret;

            // init vars
            ret = "";

            // Get the Operating System From Environment Class
            OperatingSystem os = Environment.OSVersion;

            // Get the version information
            Version vs = os.Version;

            if (os.Platform == System.PlatformID.Win32Windows)
            {
                if (vs.Major >= 4 && vs.Minor == 0)
                    ret = "WIN95";

                if (vs.Major >= 4 && (vs.Minor > 0 && vs.Minor < 90))
                    ret = "WIN98";

                if (vs.Major >= 4 && (vs.Minor > 0 && vs.Minor >= 90))
                    ret = "WINME";
            }

            if (os.Platform == System.PlatformID.Win32NT)
            {
                if (vs.Major <= 4 && vs.Minor == 0)
                    ret = "WINNT";

                if (vs.Major == 5 && (vs.Minor == 0))
                    ret = "WIN2K";

                if (vs.Major == 5 && (vs.Minor > 0))
                    ret = "WINXP";

                if (vs.Major == 6 && (vs.Minor >= 0))
                    ret = "VISTA";
            }

            // devolvemos valor
            return ret;
        }
        /// <summary>
        /// Returns MAC Address from first Network Card in Computer
        /// </summary>
        /// <returns>[string] MAC Address</returns>
        public static string GetMACAddress()
        {
            string ret = "";
            long max = 0;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                PhysicalAddress address = adapter.GetPhysicalAddress();

                if (adapter.OperationalStatus == OperationalStatus.Up && adapter.Speed > max)
                {
                    byte[] bytes = address.GetAddressBytes();
                    if (bytes.Length > 0)
                    {
                        max = adapter.Speed;
                        ret = "";
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            // Display the physical nodeAddress in hexadecimal.
                            ret += string.Format("{0}", bytes[i].ToString("X2"));
                            // Insert a hyphen after each byte, unless we are at the end of the 
                            // nodeAddress.
                            if (i != bytes.Length - 1)
                            {
                                // ret += ":";
                            }
                        }
                    }
                }
            }
            
            /*
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)  // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            */
            // MACAddress = MACAddress.Replace(":", "");
            return ret;
        }
        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        /// <returns>[string] ProcessorId</returns>
        public static string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {// only return cpuInfo from first CPU
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
            }
            return cpuInfo;
        }

        public class InfoDrive
        {
            public string unit;
            public string description;
            public string fileSystem;
            public long freeSpace;
            public long size;
        }

        public static InfoDrive[] GetInfoDrives()
        {
            WqlObjectQuery wmiquery = new WqlObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType = '3'");
            //  WHERE DeviceID = 'C:'
            ManagementObjectSearcher wmifind = new ManagementObjectSearcher(wmiquery);

            ArrayList arr = new ArrayList();
            foreach (ManagementObject mobj in wmifind.Get())
            {
                InfoDrive id = new InfoDrive();
                id.unit = mobj["Name"].ToString();
                id.fileSystem = mobj["FileSystem"].ToString();
                id.description = mobj["Description"].ToString();
                id.size = long.Parse(mobj["Size"].ToString());
                id.freeSpace = long.Parse(mobj["FreeSpace"].ToString());
                // Console.WriteLine("Description: " + mobj["Description"]);
                // Console.WriteLine("File system: " + mobj["FileSystem"]);
                // Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);
                // Console.WriteLine("Size: " + mobj["Size"]);
                arr.Add(id);
            }

            InfoDrive[] ret = new InfoDrive[arr.Count];
            arr.CopyTo(ret);

            return ret;
        }

    }
}
