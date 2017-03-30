using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace SyncService
{
    public partial class SyncService : ServiceBase
    {
        private const string sourceName = "MegalomaniaSyncServiceSource";
        private const string logName = "MegalomaniaStudiosFilesync";
        ManagementEventWatcher usbWatcher;
        WqlEventQuery usbQuery;
        public SyncService()
        {
            InitializeComponent();
            usbWatcher = new ManagementEventWatcher();
            usbQuery = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent");
            usbWatcher.EventArrived += new EventArrivedEventHandler(watcher_eventArrived);
            usbWatcher.Query = usbQuery;
            if (!EventLog.SourceExists("MegalomaniaStudiosSS"))
            {
                EventLog.CreateEventSource("MegalomaniaStudiosSS", "MegalomaniaStudios");
            }
            eventLog1.Source = "MegalomaniaStudiosSS";
            eventLog1.Log = "MegalomaniaStudios";
        }

        private void watcher_eventArrived(object sender, EventArrivedEventArgs e)
        {
            //stuff
            eventLog1.WriteEntry("Event arrived");
        }

        protected override void OnStart(string[] args)
        {
            //usbWatcher.Start();
            //eventLog1.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            usbWatcher.Stop();
        }
    }
}
