using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using autovpn_test;

namespace autovpn
{

    public partial class VpnPlugger : ServiceBase
    {
        public VpnPlugger()
        {
            InitializeComponent();

            ServiceName = "VpnPlugger";
        }

        private DriveRunner m_DriveRunner = new DriveRunner();


        protected override void OnStart(string[] args)
        {
            m_DriveRunner.Start();
        }

        protected override void OnStop()
        {
            m_DriveRunner.StopGracefully();
        }
    }
}
