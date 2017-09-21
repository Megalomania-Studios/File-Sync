using MSFileSyncer.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFileSyncer
{
    class Program
    {
        private const string relativeSyncFilePath = ".mvsfilesync\\order.sync";

        static void Main(string[] args)
        {
            //Drive letter should be first argument
            if (args.Length < 1) throw new ArgumentOutOfRangeException("drive", "No drive letter supplied.");
            var driveLetter = args[0];

            //Get sync info file and deserialize it
            var syncFilePath = Path.Combine(driveLetter, relativeSyncFilePath);
            //if theres no info file, we can't do stuff.
            if (!File.Exists(syncFilePath)) return;

            //open the file to make sure the computer considers the usb device busy
            using (var fs = File.Open(syncFilePath, FileMode.Open))
            {
                using (var sr = new StreamReader(fs))
                {
                    //read the content of the config file
                    var syncFileContent = sr.ReadToEnd();
                    var syncOrders = JsonConvert.DeserializeObject<SyncOrder[]>(syncFileContent);

                    //process all sync orders
                    foreach (var syncOrder in syncOrders)
                    {
                        ExecuteSyncOrder(driveLetter, syncOrder);
                    }
                }
            }
        }

        private static void ExecuteSyncOrder(string driveLetter, SyncOrder syncOrder)
        {
            //replace all ocurrences of $d with the current drive letter
            syncOrder.OriginFolder = syncOrder.OriginFolder.Replace("$d", driveLetter);
            syncOrder.DestinationFolder = syncOrder.DestinationFolder.Replace("$d", driveLetter);

            //creates destination folder if it doesnt exist
            Directory.CreateDirectory(syncOrder.DestinationFolder);

            //clear destination directory
            foreach (var targetDir in new DirectoryInfo(syncOrder.DestinationFolder).EnumerateDirectories())
            {
                targetDir.Delete(true);
            }

            //copy everything
            DeepCopy(syncOrder.OriginFolder, syncOrder.DestinationFolder);
        }

        private static void DeepCopy(string originFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
            var from = new DirectoryInfo(originFolder);
            var to = new DirectoryInfo(destinationFolder);
            //check for target directory being in origin directory to prevent copying over and over
            if (to.FullName.Contains(from.FullName)) throw new Exception("Folder nesting detected. Cancelling deepcopy.");

            //call this method recursively for all subdirectories
            foreach (var originDir in from.EnumerateDirectories())
            {
                var subdir = to.CreateSubdirectory(originDir.Name);
                DeepCopy(originDir.FullName, subdir.FullName);
            }

            //copy all files in this directory
            foreach (var fileInfo in from.EnumerateFiles())
            {
                fileInfo.CopyTo(Path.Combine(to.FullName, fileInfo.Name));
            }
        }
    }
}
