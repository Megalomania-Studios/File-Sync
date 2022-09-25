using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FileSyncService
{
    public partial class MegalomaniaStudiosFileSyncService : ServiceBase
    {
        ManagementEventWatcher eWatcher;

        private string exePath;
        private const string regKeyPath = @"Software\MegalomaniaStudios\";

        public MegalomaniaStudiosFileSyncService()
        {
            InitializeComponent();

            eWatcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            //volume change event, 2 means device arrival, 1 is Config changed, 3 is Device removal and 4 is Docking
            eWatcher.Query = query;
            eWatcher.EventArrived += Watcher_EventArrived;
        }

#if DEBUG
        private void Log(string msg) => File.AppendAllText(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MegalomaniaStudios\\log.txt"), msg);
#endif

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var drive = e.NewEvent.Properties["DriveName"].Value.ToString();
            //Log("\r\nevent arrived:exe path:" + (string.IsNullOrWhiteSpace(exePath) ? "empty" : exePath));
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
            proc.WaitForExit();
            //Log(proc.StandardOutput.ReadToEnd());
            //Log(proc.StandardError.ReadToEnd());
        }

        protected override void OnStart(string[] args)
        {
            //EventLog.WriteEntry("Service started.");
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_UserAccount");
                ManagementObjectCollection collection = searcher.Get();
                foreach (var user in collection)
                {
                    NTAccount f = new NTAccount((string)user.Properties["Name"].Value);
                    SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
                    var sid = s.ToString();
                    var path = (string)Registry.GetValue($"HKEY_USERS\\{sid}\\{regKeyPath}", "SyncExePath", null);
                    if (path != null) exePath = path;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Log(ex.ToString());
#endif
                throw ex;
            }
            eWatcher.Start();
        }

        protected override void OnStop()
        {

        }
    }
}
