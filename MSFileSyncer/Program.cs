using FileSyncLibrary;
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

        #region Main method
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
            SyncOrder[] syncOrders;
            using (var fs = File.Open(syncFilePath, FileMode.Open))
            {
                using (var sr = new StreamReader(fs))
                {
                    //read the content of the config file
                    var syncFileContent = sr.ReadToEnd();
                    syncOrders = JsonConvert.DeserializeObject<SyncOrder[]>(syncFileContent);

                    //process all sync orders
                    for (var i = 0; i < syncOrders.Length; i++)
                    {
                        //check whether this order should be executed
                        if (syncOrders[i].Settings.SyncType == SyncType.Always ||
                            (syncOrders[i].Settings.SyncType == SyncType.TimeSpan
                            && syncOrders[i].LastSynced.Add(syncOrders[i].Settings.SyncTime) < DateTime.Now))
                        {
                            ExecuteSyncOrder(driveLetter, syncOrders[i]);
                        }
                        //set last synced time
                        syncOrders[i].LastSynced = DateTime.Now.ToUniversalTime();
                    }
                }
            }
            File.WriteAllText(syncFilePath, JsonConvert.SerializeObject(syncOrders));
        }
        #endregion

        #region Execute sync order
        private static void ExecuteSyncOrder(string driveLetter, SyncOrder syncOrder)
        {
            //replace all ocurrences of $d with the current drive letter
            syncOrder.OriginFolder = syncOrder.OriginFolder.Replace("$d", driveLetter);
            syncOrder.DestinationFolder = syncOrder.DestinationFolder.Replace("$d", driveLetter);

            //creates destination folder if it doesnt exist
            Directory.CreateDirectory(syncOrder.DestinationFolder);

            //delete deleted files
            if (syncOrder.Settings.Delete != DoIf.Never) DeepDelete(syncOrder.OriginFolder, syncOrder.DestinationFolder, syncOrder);

            //copy everything
            DeepCopy(syncOrder.OriginFolder, syncOrder.DestinationFolder, syncOrder.Settings);

            //restore original path names
            syncOrder.OriginFolder = syncOrder.OriginFolder.Replace(driveLetter, "$d");
            syncOrder.DestinationFolder = syncOrder.DestinationFolder.Replace(driveLetter, "$d");
        }
        #endregion

        #region DeepCopy
        private static void DeepCopy(string originFolder, string destinationFolder, SyncRules settings)
        {
            //create the directory if necessary
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder/*, Directory.GetAccessControl(originFolder)*/);
            //get directory infos
            var from = new DirectoryInfo(originFolder);
            var to = new DirectoryInfo(destinationFolder);
            //check for target directory being in origin directory to prevent copying over and over
            if (to.FullName.Contains(from.FullName)) throw new Exception("Folder nesting detected. Cancelling deepcopy.");

            //call this method recursively for all subdirectories
            foreach (var originDir in from.EnumerateDirectories())
            {
                var subdir = new DirectoryInfo(Path.Combine(to.FullName, originDir.Name));
                if (!subdir.Exists)
                {
                    subdir = to.CreateSubdirectory(originDir.Name/*, originDir.GetAccessControl()*/);
                }
                DeepCopy(originDir.FullName, subdir.FullName, settings);
            }

            //copy all files in this directory
            foreach (var fileInfo in from.EnumerateFiles())
            {
                string filePath = Path.Combine(to.FullName, fileInfo.Name);
                var targetFileInfo = new FileInfo(filePath);
                //check override setting
                switch (settings.Override)
                {
                    //allow overriding in any case
                    case DoIf.Always:
                        fileInfo.CopyTo(filePath, true);
                        break;
                    //never allow overwriting
                    case DoIf.Never:
                        if (!targetFileInfo.Exists) fileInfo.CopyTo(filePath);
                        break;
                    //allow overwriting if the target file is older than the source file
                    case DoIf.OnlyNewer:
                    //this is also the default case
                    default:
                        if (File.Exists(filePath))
                        {
                            if (targetFileInfo.LastWriteTimeUtc < fileInfo.LastWriteTimeUtc) fileInfo.CopyTo(filePath, true);
                        }
                        else
                        {
                            fileInfo.CopyTo(filePath);
                        }
                        break;
                }
            }
        }
        #endregion

        #region DeepDelete
        private static void DeepDelete(string originFolderPath, string destinationFolderPath, SyncOrder syncOrder)
        {
            //get directory info
            var originInfo = new DirectoryInfo(originFolderPath);
            var destinationInfo = new DirectoryInfo(destinationFolderPath);
            if (!destinationInfo.Exists) return;

            //cycle through folders to find deleted ones
            var originDirectories = originInfo.EnumerateDirectories();
            var destinationDirectories = destinationInfo.EnumerateDirectories();
            //go through all folders found in destination but not origin
            foreach (var directoryToDelete in destinationDirectories)
            {
                if (originDirectories.Any((x) => x.Name == directoryToDelete.Name))
                {
                    DeepDelete(Path.Combine(originFolderPath, directoryToDelete.Name), directoryToDelete.FullName, syncOrder);
                }
                else if (syncOrder.Settings.Delete == DoIf.Always || directoryToDelete.LastWriteTimeUtc < syncOrder.LastSynced)
                {
                    directoryToDelete.Delete(true);
                }
            }

            //now go through all files
            var originFiles = originInfo.EnumerateFiles();
            var destinationFiles = destinationInfo.EnumerateFiles();
            //go through all files found in destination but not origin
            foreach (var fileToDelete in destinationFiles.Where((x) => !originFiles.Any((y) => y.Name == x.Name)))
            {
                if (syncOrder.Settings.Delete == DoIf.Always || fileToDelete.LastWriteTimeUtc < syncOrder.LastSynced)
                {
                    fileToDelete.Delete();
                }
            }
        }
        #endregion
    }
}
