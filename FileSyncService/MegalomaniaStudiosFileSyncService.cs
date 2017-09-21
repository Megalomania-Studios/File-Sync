using Microsoft.Win32;
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
using System.Threading.Tasks;

namespace FileSyncService
{
    public partial class MegalomaniaStudiosFileSyncService : ServiceBase
    {
        ManagementEventWatcher eWatcher;

        private string exePath;
        private const string regKeyPath = @"HKEY_CURRENT_USER\Software\MegalomaniaStudios\";

        public MegalomaniaStudiosFileSyncService()
        {
            InitializeComponent();
            
            eWatcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            //volume change event, 2 means device arrival, 1 is Config changed, 3 is Device removal and 4 is Docking
            eWatcher.Query = query;
            eWatcher.EventArrived += Watcher_EventArrived;
        }

        private void Log(string msg) => File.AppendAllText("C:\\Users\\Florian\\Desktop\\log.txt", msg);

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            exePath = (string)Registry.GetValue(regKeyPath, "SyncExePath", "it failed");
            var drive = e.NewEvent.Properties["DriveName"].Value.ToString();
            File.AppendAllText("C:\\Users\\Florian\\Desktop\\log.txt", "\r\nevent arrived:exe path:" +
                (string.IsNullOrWhiteSpace(exePath) ? "empty" : exePath));
            var psi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = exePath,
                Arguments = drive,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var proc = Process.Start(psi);
        }

        protected override void OnStart(string[] args)
        {
            //EventLog.WriteEntry("Service started.");
            exePath = (string)Registry.GetValue(regKeyPath, "SyncExePath", "it failed");
            eWatcher.Start();
        }

        protected override void OnStop()
        {

        }
    }
}
