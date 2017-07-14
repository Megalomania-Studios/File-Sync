﻿using Megalomania_Studios_Filesync.Properties;
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

namespace Megalomania_Studios_Filesync
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //läd den Status des Backups neu, zeigt aktiv oder inaktiv und den jeweils richtigen Button an. Läd außerdem die Geräteliste neu und SOLL auch die Ordnerliste neu laden.
        public void ReloadState ()
        {
            Deviceact();
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
            Folderact();
        }
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
                //hier und auch sonst später den (noch kommenden) Errorcodehandler (Methode, der man den Errorcode zum Fraß vorwirft) konsultieren
                MessageBox.Show("Ein Fehler ist aufgetreten. (Code: 0x00002) Der Status des Backups konnte nicht erkannt werden.", "Fehler bei der Erkennung des Backupzustandes");
            return;
        }

        public void Deviceact ()
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

                //items.Add(new Devices() { DLetter = "F://", Name = " "+ "USB"});

                //items.Add(new Devices() { DLetter = "Usbgerät (G://)" });
                Devices.ItemsSource = items;

        }

        public void Folderact ()
        {
            List<Folders> items = new List<Folders>();
            items.Add(new Folders() { OriginFolder = "Origin", DestinationFolder = "Destiny", SyncTime = "SyncTime" });
            Folders.ItemsSource = items;
        }


        public MainWindow()
        {
            
            
            InitializeComponent();
            BackupIsActivated = false;
            //Status des Backups und Geräteliste laden und Darstellen
            ReloadState();
            this.DataContext = fold;
            
            




            //if (!Settings.Default.HasBeenInstalled)
            {
                //install();
            }
            //ServiceController service = new ServiceController("SyncService");
            //service.Start();

        }

        Folders fold = new Folders { OriginFolder = "Mannfred", DestinationFolder = "5", SyncTime = "Köln" };


        private void install()
        {
            //throw new NotImplementedException();
        }

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

        public bool BackupIsActivated { get; set; }

        private void ListBoxItem_Selected(object sender, EventArgs e)
        {
            //hier muss dann die Datenbankabfrage für das entsprechende Gerät implementiert werden
        }

        private void Devices_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(Devices.SelectedIndex.ToString());
        }

        private void Folders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    //wichtig für die Geräteliste
    public class Devices
    {
        public string DLetter { get; set; }
        public string Name { get; set; }
        
    }
    public class Folders
    {
        public string OriginFolder { get; set; }
        public string DestinationFolder { get; set; }
        public string SyncTime { get; set; }
    }
}
