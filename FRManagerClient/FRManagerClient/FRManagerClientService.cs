using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FRManagerClient
{
    public partial class FRManagerClientService : ServiceBase
    {
        private Timer timer = null;

        public FRManagerClientService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            this.timer.Interval = 7*1000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_ticked);
            timer.Enabled = true;
            Speak.greeting();
            GetPerformance.Get();
        }

        private void timer_ticked(object sender, ElapsedEventArgs e)
        {
            GetPerformance.Get();
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }
    }
}
