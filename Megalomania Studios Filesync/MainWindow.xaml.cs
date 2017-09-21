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
using windowsforms = System.Windows.Forms;



namespace Megalomania_Studios_Filesync
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>

    #region Main class
    public partial class MainWindow : Window
    {

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

        //läd den Status des Backups neu, zeigt aktiv oder inaktiv und den jeweils richtigen Button an. Läd außerdem die Geräteliste neu und SOLL auch die Ordnerliste neu laden.
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
            //hier statt Wertabfrage der schon deklarierten variablen (zu testzwecken) Abfragemechanismus einbauen
            bool backacttest = BackupIsActivated;
            //Diese If-bedingungen sollen die Aktivität des Dienstes überprüfen, momentan überprüfen sie nur die Variable, in die sie wieder reinschreiben (also ist es bisher quatsch)
            if (backacttest == true)
            {
                //hier check für dienst läuft/läuft nicht einfügen
                BackupIsActivated = true;
                return;
            }

            if (backacttest == false)
            {
                //hier auch
                BackupIsActivated = false;
                return;
            }
            else
                //falls irgendwas schiefläuft
                BackupIsActivated = false;
            //hier und auch sonst später den (noch kommenden) Errorcodehandler (Methode, der man den Errorcode zum Fraß vorwirft ;-)) konsultieren
            MessageBox.Show("Ein Fehler ist aufgetreten. (Code: 0x00002) Der Status des Backups konnte nicht erkannt werden.", "Fehler bei der Erkennung des Backupzustandes");
            return;
        }
        #endregion

            #region Deviceact (Update of connected devices)

        public void Deviceact()
        {
            //Läd alle angeschlossenen Festplatten mit Name. Es geht auch mit Win32, aber so funktioniert es zuverlässiger (bzw. überhaupt erst :-)). Erkennt alles, was Diskpart als "online" ansieht, auch USB-Geräte, SDKarten (beides ausprobiert), Floppys (nicht selbst getestet :-))

            List<Devices> items = new List<Devices>();

            DriveInfo[] laufwerke = DriveInfo.GetDrives();


            foreach (DriveInfo driveinfo in laufwerke)
            {
                //Falls irgendwas mit irgendeinem Laufwerk nicht stimmt, beispielsweise nichterkanntes Dateiformat o.ä.
                try
                {
                    items.Add(new Devices() { DLetter = " " + driveinfo.VolumeLabel, Name = driveinfo.Name });
                }

                catch (IOException ex)
                {

                    MessageBox.Show(ex.ToString());
                }

            }

            Devices.ItemsSource = items;

            Folderact();

        }
        #endregion

            #region  Folderact (Update of Folders based on file?)

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
                //string Laufwerksbuchstabe = Devices.FindName(Devicescuritem).ToString();
                MessageBox.Show(Devicescuritem);
                MessageBox.Show(File.Exists(Path.Combine(Devicescuritem, "test.txt")).ToString());
                if (!File.Exists(Path.Combine(Devicescuritem, "test.txt")))
                {
                    Folders.IsEnabled = false;
                    MessageBox.Show("Errungenschaft freigeschaltet: Die M0n0t0nie des Lebens! Keine Datei zum auslesen gefunden!");
                    Foldersource.Clear();

                    Foldersource.Add(new Folders() { OriginFolder = "Keine Ordner gefunden" });

                    
                    return;
                }
                else
                {

                    Folders.IsEnabled = true;
                    Foldersource.Clear();
                    string folders = File.ReadAllText(Path.Combine(Devicescuritem + "test.txt"));
                    var list = JsonConvert.DeserializeObject<List<Folders>>(folders);

                    foreach (var f in list)
                    {
                        Foldersource.Add(f);
                    }                  

                }
            }
        }

        #endregion

            #region installer (work in Progress)

        private void install()
        {
            //throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Clickhandlers

        #region selectionchanged methods
        #region Backupstate Button ((de)activation)
        private void BackupstateChange_Click(object sender, RoutedEventArgs e)
        {

            Backact();
            //hier muss natürlich noch der Dienst gestartet oder gestoppt werden und auch der Autostart aktiviert oder deaktiviert werden
            if (BackupIsActivated == true)
            {
                // das kommmt dann natürlich weg
                BackupIsActivated = false;

                ReloadState();
                return;

            }
            if (BackupIsActivated == false)
            {
                //das kommt dann natürlich weg
                BackupIsActivated = true;

                ReloadState();
                return;
            }
        }
        #endregion



        private void Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Folderact();
            
        }

        private void Folders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region button_clicks

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
                MessageBox.Show(Devicescuritem);
                var folders = Folders.ItemsSource;
                var serialized = JsonConvert.SerializeObject(folders);
                File.WriteAllText(Path.Combine(Devicescuritem + "test.txt"), serialized);
            }

        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Deviceact();
        }

        private void AddNewPair_Click(object sender, RoutedEventArgs e)
        {
            if (((Devices)Devices.SelectedItem) == null)
            {
                MessageBox.Show("Kein Gerät ausgewählt! Bitte wählen sie zunächst ein Gerät", "Fehler");
                return;
            }
            else
            {
                
                var folderdialog = new windowsforms.FolderBrowserDialog();
                folderdialog.Description = "Ursprungsordner auswählen";
                folderdialog.ShowNewFolderButton = false;
                folderdialog.ShowDialog();
                string originpath = folderdialog.SelectedPath;
                folderdialog.Reset();
                folderdialog.ShowNewFolderButton = false;
                folderdialog.Description= "Zielordner auf dem Backup-Gerät auswählen";
                folderdialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderdialog.ShowDialog();
                string destinationpath = folderdialog.SelectedPath;               
                Foldersource.Add(new Folders { OriginFolder = originpath, DestinationFolder = destinationpath, SyncTime = "immer" });

            }
       

    }
        private void DeletePair_Click(object sender, RoutedEventArgs e)
        {
            if (((Devices)Devices.SelectedItem) == null)
            {
                MessageBox.Show("Kein Gerät ausgewählt! Bitte wählen sie zunächst ein Gerät", "Fehler");
                return;
            }
            else
            {
                Foldersource.Remove(((Folders)Folders.SelectedItem));
            }
        }

        #endregion

        #endregion

        #region class-internal variables

        private ObservableCollection<Folders> Foldersource = new ObservableCollection<Folders>();

        public bool BackupIsActivated { get; set; }




        #endregion

      
    }
    #endregion

    #region variables in independent classes

    public class Devices
    {
        public string DLetter { get; set; }
        public string Name { get; set; }

    }
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Folders
    {
        [JsonProperty(PropertyName = "origin_folder", Required = Required.Always)]
        public string OriginFolder { get; set; }
        [JsonProperty(PropertyName = "destination_folder", Required = Required.Always)]
        public string DestinationFolder { get; set; }
        [JsonProperty(PropertyName = "sync_time", Required = Required.Always)]
        public string SyncTime { get; set; }
    }
    #endregion
}
