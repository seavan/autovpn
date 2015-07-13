using System;
using System.Collections.Generic;
using System.Text;

namespace autovpn_test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dc = new DriveRunner();
            dc.Start();


            if (Console.Read() > 0)
            {
                dc.StopGracefully();
            }
        }
    }
}
