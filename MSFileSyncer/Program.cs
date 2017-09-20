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
            if (!File.Exists(syncFilePath)) return;
            var syncFileContent = File.ReadAllText(syncFilePath);
            var syncOrder = JsonConvert.DeserializeObject<SyncOrder>(syncFileContent);
            //replace all ocurrences of $d with the current drive letter
            syncOrder.OriginFolder = syncOrder.OriginFolder.Replace("$d", driveLetter);
            syncOrder.DestinationFolder = syncOrder.DestinationFolder.Replace("$d", driveLetter);

            DeepCopy(syncOrder.OriginFolder, syncOrder.DestinationFolder);
        }

        private static void DeepCopy(string originFolder, string destinationFolder)
        {
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
