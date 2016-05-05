using log4net;
using OutDiskReadService.APP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace OutDiskReadService
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Service1));
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TMStart.Enabled = true;
        }

        protected override void OnStop()
        {

        }

        private void TMStart_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TMStart.Enabled = false;
            _log.Info("服务运行");

            DiskFileRead dfr = new DiskFileRead();
            dfr.start();
            _log.Info("\r\n-------------------------------读取服务-------------------------------");
        }
    }
}
