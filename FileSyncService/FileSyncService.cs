using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSyncService
{
    public partial class MegalomaniaStudiosFileSyncService : ServiceBase
    {
        ManagementEventWatcher eWatcher;
        private static readonly string appDataPath = 
            Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), "Megalomania Studios\\");
        public MegalomaniaStudiosFileSyncService()
        {
            InitializeComponent();
            eWatcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            //volume change event, 2 means device arrival, 1 is Config changed, 3 is Device removal and 4 is Docking
            eWatcher.Query = query;
            eWatcher.EventArrived += Watcher_EventArrived;
        }

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var drive = e.NewEvent.Properties["DriveName"].Value;
            File.AppendAllText("C:\\Users\\Florian\\Desktop\\test.txt", Environment.NewLine + drive);
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service started.");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
            eWatcher.Start();
        }

        protected override void OnStop()
        {
            
        }
    }
}
