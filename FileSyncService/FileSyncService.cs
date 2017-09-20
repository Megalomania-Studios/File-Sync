using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSyncService
{
    public partial class MegalomaniaStudiosFileSyncService : ServiceBase
    {
        ProcessStartInfo psi;
        Process proc;
        private static readonly string appDataPath = 
            Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), "Megalomania Studios\\");
        public MegalomaniaStudiosFileSyncService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service started.");
            if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);
            var batPath = Path.Combine(appDataPath, "test.bat");
            File.WriteAllText(batPath, "echo HELLO WORLD");
            psi = new ProcessStartInfo
            {
                CreateNoWindow = false,
                FileName = "cmd.exe",
                Arguments = "/C " + batPath,
                WindowStyle = ProcessWindowStyle.Normal,
            };
            proc = Process.Start(psi);
        }

        protected override void OnStop()
        {
            proc.Close();
        }
    }
}
