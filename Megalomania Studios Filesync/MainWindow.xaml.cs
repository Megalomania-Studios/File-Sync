using Megalomania_Studios_Filesync.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.IO;
using System.Collections.ObjectModel;
using Path = System.IO.Path;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Microsoft.Win32;
using FileSyncLibrary;
using windowsforms = System.Windows.Forms;



namespace Megalomania_Studios_Filesync
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    
    // TODO: Custom title bar
    // TODO: Custom message boxes
    // TODO: Make sync file path constant depend on sync file folder constant

    #region Main class

    public partial class MainWindow : Window
    {

        private const string noFoldersFound = "Keine Ordner gefunden";

        #region Constructor

        public MainWindow()
        {


            InitializeComponent();
            BackupIsActivated = false;
            //Status des Backups und Geräteliste laden und Darstellen
            ReloadState();
            //this.DataContext = fold;


            //if (!Settings.Default.HasBeenInstalled)
            {
                //install();
            }
            //ServiceController service = new ServiceController("SyncService");
            //service.Start();
        }

        #endregion

        #region Update methods

        #region ReloadState (Update)

        //reloads the lists and activation state of the backup Service
        public void ReloadState()
        {
            Folders.ItemsSource = Foldersource;
            Deviceact();
            Folderact();
            Backact();
            if (BackupIsActivated == true)
            {
                BackupstateState.Content = "aktiviert";
                BackupstateChange.Content = "deaktivieren";
                return;
            }
            if (BackupIsActivated == false)
            {
                BackupstateState.Content = "deaktiviert";
                BackupstateChange.Content = "aktivieren";
                return;
            }

        }
        #endregion

        #region Backact (is Backup activated?)
        public void Backact()
        {
            
            bool backacttest = BackupIsActivated;
            //at the moment static (for testing purposes)
            if (backacttest == true)
            {
                
                BackupIsActivated = true;
                return;
            }

            if (backacttest == false)
            {
              
                BackupIsActivated = false;
                return;
            }
            else
                // if something goes wrong, better tell its deactiveted and show a message
                BackupIsActivated = false;
            //in the future: errorcodehandler-method
            MessageBox.Show("Ein Fehler ist aufgetreten. (Code: 0x00002) Der Status des Backups konnte nicht erkannt werden.", "Fehler bei der Erkennung des Backupzustandes");
            return;
        }
        #endregion

        #region Deviceact (Update of connected devices)

        public void Deviceact()
        {
            //Loads all connected (flash)drives, sdcards, floppy discs, etc, everything that diskpart consideres "online"
            List<Devices> items = new List<Devices>();

            DriveInfo[] drives= DriveInfo.GetDrives();


            foreach (DriveInfo driveinfo in drives)
            {
                //if something w/ the detection goes wrong (bad drive, unformatted drive)
                try
                {
                    items.Add(new Devices() { DLetter = " " + driveinfo.VolumeLabel, Name = driveinfo.Name });
                }

                catch (IOException ex)
                {
#if DEBUG
                    MessageBox.Show(ex.ToString());
#else
                    throw ex;
#endif
                }

            }

            Devices.ItemsSource = items;

            Folderact();

        }
        #endregion

        #region  Folderact (Update of Folders based on file)

        //Method calls Devices.Selecteditems to identify the selected device. Then loads the file and extracts the folders from it, displays them afterwards

        public void Folderact()
        {


            if (((Devices)Devices.SelectedItem) == null)
            {
                Foldersource.Clear();
                Foldersource.Add(new Folders() { OriginFolder = "Kein Gerät ausgewählt" });
                Folders.IsEnabled = false;
                return;
            }
            else
            {

                string Devicescuritem = ((Devices)Devices.SelectedItem).Name;

                string dir = Path.Combine(Devicescuritem, ".mvsfilesync\\");
                if (!Directory.Exists(dir))
                {
                    var dirInfo = Directory.CreateDirectory(dir);
                    dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }

                if (!File.Exists(Path.Combine(Devicescuritem, relativeSyncFilePath)))
                {
                    Folders.IsEnabled = false;
                    
                    Foldersource.Clear();

                    Foldersource.Add(new Folders() { OriginFolder = noFoldersFound });
                    
                    return;
                }
                else
                {

                    

                    Foldersource.Clear();
                    string folders = File.ReadAllText(Path.Combine(Devicescuritem + relativeSyncFilePath));

                    if (folders == "[]")
                    {
                        Foldersource.Add(new Folders() { OriginFolder = noFoldersFound });
                        Folders.IsEnabled = false;
                        return;

                    }

                    else
                    {
                        Folders.IsEnabled = true;

                        var list = JsonConvert.DeserializeObject<List<SyncOrder>>(folders);

                        foreach (var f in list)
                        {
                            Foldersource.Add(f);
                        }
                        return;
                    }
                }
            }
        }

        #endregion



        #endregion

        #region Clickhandlers

        #region Closebutton
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        #region Minimizebutton
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion

        #region Backupstate Button ((de)activation)

        //for starting/stopping the service

        private void BackupstateChange_Click(object sender, RoutedEventArgs e)
        {

            Backact();
            
            if (BackupIsActivated == true)
            {
                // temporary
                BackupIsActivated = false;

                ReloadState();
                return;

            }
            if (BackupIsActivated == false)
            {
                //temporary, too
                BackupIsActivated = true;

                ReloadState();
                return;
            }
        }
        #endregion

        #region Device selected
        private void Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Folderact();

        }
        #endregion

        #region Save button
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (((Devices)Devices.SelectedItem) == null)
            {
                MessageBox.Show("Kein Gerät ausgewählt! Bitte wählen sie zunächst ein Gerät", "Fehler");
                return;
            }
            else
            {
                string Devicescuritem = ((Devices)Devices.SelectedItem).Name;
                var folders = Folders.ItemsSource;
                var orders = new List<SyncOrder>();
                foreach (Folders f in folders)
                {
                    if (f.OriginFolder == null || f.DestinationFolder == null) continue;
                    orders.Add(new SyncOrder(f.OriginFolder, f.DestinationFolder, SyncRules.Default));
                }
                var serialized = JsonConvert.SerializeObject(orders);
                if (!Directory.Exists(Path.Combine(Devicescuritem, relativeSyncFileFolder)))
                {
                    var di = Directory.CreateDirectory(Path.Combine(Devicescuritem, relativeSyncFileFolder));
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
                File.WriteAllText(Path.Combine(Devicescuritem, relativeSyncFilePath), serialized);
                Folderact();
            }

        }
        #endregion

        #region reload button
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Deviceact();
        }
        #endregion

        #region add button
        private void AddNewPair_Click(object sender, RoutedEventArgs e)
        {
            if (((Devices)Devices.SelectedItem) == null)
            {
                MessageBox.Show("Kein Gerät ausgewählt! Bitte wählen sie zunächst ein Gerät", "Fehler");
                return;
            }
            else
            {
                var driveLetter = ((Devices)Devices.SelectedItem).Name;
                var folderdialog = new windowsforms.FolderBrowserDialog
                {
                    Description = "Ursprungsordner auswählen"
                };
                var result = folderdialog.ShowDialog();
                if (result != windowsforms.DialogResult.OK) return;
                string originpath = folderdialog.SelectedPath;
                folderdialog.Reset();
                folderdialog.Description = "Zielordner auf dem Backup-Gerät auswählen";
                folderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
                result = folderdialog.ShowDialog();
                if (result != windowsforms.DialogResult.OK) return;
                string destinationpath = folderdialog.SelectedPath;
                string d = "$d\\";

                destinationpath = destinationpath.Replace(driveLetter, d);
                originpath = originpath.Replace(driveLetter, d);
                Foldersource.Add(new Folders { OriginFolder = originpath, DestinationFolder = destinationpath,
                    SyncTime = SyncType.Always.ToString(), Settings = SyncRules.Default });
                var message = Foldersource.FirstOrDefault((x) => x.OriginFolder == noFoldersFound);
                if (message != null) Foldersource.Remove(message);
                Folders.IsEnabled = true;
                
            }


        }
        #endregion

        #region remove button
        private void DeletePair_Click(object sender, RoutedEventArgs e)
        {
            if (((Devices)Devices.SelectedItem) == null)
            {
                MessageBox.Show("Kein Gerät ausgewählt! Bitte wählen sie zunächst ein Gerät", "Fehler");
                return;
            }
            else
            {
                var Boxresult = MessageBox.Show("Das Ordnerpaar wirklich löschen?", "Filesync", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.None);
                if (Boxresult == MessageBoxResult.Yes)
                {
                    Foldersource.Remove(((Folders)Folders.SelectedItem));
                    
                    
                    return;
                }
                else
                {                    
                    return;
                }
            }
        }
        #endregion


        #endregion

        

        #region class-internal variables

        private ObservableCollection<Folders> Foldersource = new ObservableCollection<Folders>();

        public bool BackupIsActivated { get; set; }
        
        private const string relativeSyncFilePath = ".mvsfilesync\\order.sync";

        private const string relativeSyncFileFolder = ".mvsfilesync\\";

        #endregion


    }
    #endregion

    #region variables in independent classes

    //storing the Devices (Name and Device-Letter)

    public class Devices
    {
        public string DLetter { get; set; }
        public string Name { get; set; }

    }

    //Storing the Folders (w/ JSON Serialization-Capability)

    //[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Folders
    {
        //[JsonProperty(PropertyName = "origin_folder", Required = Required.Always)]
        public string OriginFolder { get; set; }
        //[JsonProperty(PropertyName = "destination_folder", Required = Required.Always)]
        public string DestinationFolder { get; set; }
        //[JsonProperty(PropertyName = "sync_time", Required = Required.Always)]
        public string SyncTime { get; set; }
        public SyncRules Settings { get; set; }

        public static implicit operator Folders(SyncOrder order)
        {
            return new Folders
            {
                DestinationFolder = order.DestinationFolder,
                OriginFolder = order.OriginFolder,
                SyncTime = order.Settings.SyncType.ToString(),
                Settings = order.Settings
            };
        }
    }
    #endregion
}