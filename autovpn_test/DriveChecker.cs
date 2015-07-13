using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace autovpn_test
{
    public class DriveChecker
    {
        public DriveChecker()
        {

        }

        /// <summary>
        /// Retuns list of all Removable drives aailable
        /// </summary>
        /// <param name="UsbDrives">Structure to store drive names (e.g. "D:", "E:" etc.</param>
        /// <returns>true if at least one removable is available</returns>
        public bool GetRemovableDisks(out List<string> UsbDrives)
        {
            // Add System.Management reference. Then
            bool result = false;
            UsbDrives = new List<string>();
            using (System.Management.ManagementClass managementClass = new System.Management.ManagementClass("Win32_Diskdrive"))
            {
                using (System.Management.ManagementObjectCollection driveCollection = managementClass.GetInstances())
                {
                    foreach (System.Management.ManagementObject driveObject in driveCollection)
                        foreach (System.Management.ManagementObject drivePartition in driveObject.GetRelated("Win32_DiskPartition"))
                            foreach (System.Management.ManagementBaseObject logicalDisk in drivePartition.GetRelated("Win32_LogicalDisk"))
                            {
                                string drive = (logicalDisk["Name"]).ToString();
                                string driveDescription = logicalDisk.Properties["Description"].Value.ToString();
                                if (driveDescription.Equals("Removable Disk", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    UsbDrives.Add(drive);
                                    result = true;
                                }
                            }
                }
            }
            return result;
        }


        public string DoCheck()
        {
            if (IsVpnRunning())
            {
                return null;
            }

            List<string> list = null;

            GetRemovableDisks(out list);

            foreach (var Drive in list)
            {
                Console.WriteLine(Drive);
                //if (Drive.DriveType == DriveType.Removable)
                {
                    //Add to RemovableDrive list or whatever activity you want
                    var configs = Directory.GetFiles(Drive + "\\", "*.ovpn");
                    if (configs.Length > 0)
                    {
                        
                        var fname = configs.First();
                        Console.WriteLine("Found something {0}", fname);
                        return fname;
                    }
                }
            }

            return null;
        }

        public Process StartOpenVpn(string _config)
        {
            KillOpenVpn();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Path.GetFullPath(_config);
            startInfo.FileName = OpenVpnPath;
            startInfo.Arguments = _config;

            CurrentProcess = Process.Start(startInfo);
            CurrentFilePath = _config;
            return CurrentProcess;
        }

        public bool DoCheckRemove()
        {
            if (!String.IsNullOrEmpty(CurrentFilePath) && (CurrentProcess != null))
            {
                if (!File.Exists(CurrentFilePath))
                {
                    return true;
                }
            }

            return false;
        }

        public void KillOpenVpn()
        {
            if ((CurrentProcess != null))
            {
                CurrentProcess.Kill();
                CurrentProcess.WaitForExit(1000);
                CurrentProcess = null;
            }
        }

        public bool IsVpnRunning()
        {
            return CurrentProcess != null;
        }


        public string OpenVpnPath { get; set; }
        public string LookForFile { get; set; }
        public Process CurrentProcess { get; set; }
        public string CurrentFilePath { get; set; }
    }
}