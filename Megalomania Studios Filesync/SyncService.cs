using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Megalomania_Studios_Filesync
{
    partial class SyncService : ServiceBase
    {
        ManagementEventWatcher watcher;
        public SyncService()
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("MegalomaniaSyncServiceSource"))
            {
                EventLog.CreateEventSource("MegalomaniaSyncServiceSource", "MegalomaniaStudiosFilesync");
            }
            eventLog1.Source = "MegalomaniaSyncServiceSource";
            eventLog1.Log = "MegalomaniaStudiosFilesync";
            watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            watcher.EventArrived += new EventArrivedEventHandler(Watcher_EventArrived);
            watcher.Query = query;
        }

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            eventLog1.WriteEntry("Event fired. " + e.NewEvent.Properties["DriveName"].Value.ToString());
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Started Service.");
            watcher.Start();
        }

        protected override void OnStop()
        {
            watcher.Stop();
        }
    }
}
