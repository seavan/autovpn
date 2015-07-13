using System;
using System.IO;
using System.Collections.Generic;

namespace autovpn_test
{
    public class DriveRunner : ThreadWorker
    {
        public DriveRunner()
        {
            CycleTime = 1000;
            m_DC.OpenVpnPath = @"C:\Program Files (x86)\OpenVPN\bin\openvpn.exe";

            if (!File.Exists(m_DC.OpenVpnPath))
            {
                m_DC.OpenVpnPath = @"C:\Program Files\OpenVPN\bin\openvpn.exe";
            }

        }

     


        public override void DoJob()
        {
            base.DoJob();

            if (!File.Exists(m_DC.OpenVpnPath))
            {
                return;
            }



            if (m_DC.IsVpnRunning())
            {
                Console.WriteLine("Checking if still plugged");
                if (m_DC.DoCheckRemove())
                {
                    Console.WriteLine("Was removed. Killing vpn");
                    m_DC.KillOpenVpn();
                    return;
                }
            }

            Console.WriteLine("Checking if something plugged");


            var newConfig = m_DC.DoCheck();

            if (!String.IsNullOrEmpty(newConfig))
            {
                Console.WriteLine("Plugged in {0}", newConfig);
                m_DC.StartOpenVpn(newConfig);
            }
        }

        private DriveChecker m_DC = new DriveChecker();
    }
}