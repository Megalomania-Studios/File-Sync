using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace FileSyncService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

#pragma warning disable IDE1006 // Benennungsstile
        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
#pragma warning restore IDE1006 // Benennungsstile
        {
            SetInterActWithDeskTop();
        }

        private static void SetInterActWithDeskTop()
        {
            var service = new System.Management.ManagementObject(
                    String.Format("WIN32_Service.Name='{0}'", "MegalomaniaStudiosFileSyncService"));
            try
            {
                var paramList = new object[11];
                paramList[5] = true;
                service.InvokeMethod("Change", paramList);
            }
            finally
            {
                service.Dispose();
            }
        }

    }
}
