﻿using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HCI_Projekat.KlijentView
{
    /// <summary>
    /// Interaction logic for PregledManifestacije.xaml
    /// </summary>
    public partial class PregledManifestacije : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public ObservableCollection<Ponuda> Ponude { get; set; }
        public Manifestacija _manifestacija;
        public Manifestacija Manifestacija
    {
            get
            {
                return _manifestacija;
            }
            set
            {
                if (value != _manifestacija)
                {
                    _manifestacija = value;
                    OnPropertyChanged("Manifestacija");
                }
            }
        }
        public DataGrid ParentData { get; set; }
        public Korisnik Klijent { get; set; }

        public PregledManifestacije(Manifestacija m, DataGrid data, Korisnik k)
        {
            InitializeComponent();
            DataContext = this;
            Manifestacija = m;
            Klijent = k;
            ParentData = data;
            using (var db = new DatabaseContext())
            {
                List<Ponuda> ponude = (from man in db.Manifestacije where man.Id == m.Id select man.Ponude).ToArray()[0];
                Ponude = new ObservableCollection<Ponuda>(ponude);
            }
            if (Manifestacija.MestoOdrzavanjaDone && Manifestacija.BudzetDone && Manifestacija.TemaDone && Manifestacija.GostiDone && Manifestacija.RasporedDone && Manifestacija.DekoracijaDone && Manifestacija.MuzikaDone && Manifestacija.DodatnoDone && Manifestacija.DatumDone && Manifestacija.Status == StatusManifestacije.U_IZRADI)
            {
                odobri.IsEnabled = true;
            }
            if (Manifestacija.Status == StatusManifestacije.ZAVRSENA)
            {
                sacuvaj.IsEnabled = false;
                otkazi.IsEnabled = false;
            }
        }

        public void Nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda zelite da sacuvate promene?", 0);
            wk.ShowDialog();

            if (wk.Result == MessageBoxResult.Yes)
            {
                using (var db = new DatabaseContext())
                {
                    Manifestacija stara = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).ToArray()[0];
                    if (stara.MestoOdrzavanja != Manifestacija.MestoOdrzavanja)
                    {
                        stara.MestoOdrzavanja = Manifestacija.MestoOdrzavanja;
                        stara.MestoOdrzavanjaDone = false;
                        mestoCheck.IsChecked = false;
                        odobri.IsEnabled = false;
                    }
                    if (stara.Budzet != Manifestacija.Budzet)
                    {
                        stara.Budzet = Manifestacija.Budzet;
                        stara.BudzetDone = false;
                        budzetCheck.IsChecked = false;
                        odobri.IsEnabled = false;
                    }
                    if (stara.Dekoracija != Manifestacija.Dekoracija)
                    {
                        stara.Dekoracija = Manifestacija.Dekoracija;
                        stara.DekoracijaDone = false;
                        dekoracijaCheck.IsChecked = false;
                        odobri.IsEnabled = false;
                    }
                    if (stara.Muzika != Manifestacija.Muzika)
                    {
                        stara.Muzika = Manifestacija.Muzika;
                        stara.MuzikaDone = false;
                        muzikaCheck.IsChecked = false;
                        odobri.IsEnabled = false;
                    }
                    if (stara.DodatniZahtevi != Manifestacija.DodatniZahtevi)
                    {
                        stara.DodatniZahtevi = Manifestacija.DodatniZahtevi;
                        stara.DodatnoDone = false;
                        dodatnoCheck.IsChecked = false;
                        odobri.IsEnabled = false;
                    }
                    if (stara.DatumOdrzavanja != Manifestacija.DatumOdrzavanja)
                    {
                        stara.DatumOdrzavanja = Manifestacija.DatumOdrzavanja;
                        stara.DatumDone = false;
                        datumCheck.IsChecked = false;
                        odobri.IsEnabled = false;
                    }
                    db.SaveChanges();
                }
            }
        }

        public void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda zelite da otkazete \nmanifestaciju?", 5);
            wk.ShowDialog();

            if (wk.Result == MessageBoxResult.Yes)
            {
                using (var db = new DatabaseContext())
                {
                    Manifestacija toDelete = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).ToArray()[0];
                    toDelete.Obrisana = true;
                    db.SaveChanges();
                }
                using (var db = new DatabaseContext())
                {
                    List<Manifestacija> manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Obrisana != true select m).ToList();
                    ParentData.ItemsSource = new ObservableCollection<Manifestacija>(manifestations);
                }
                this.Close();
            }
        }

        public void Odobri_Click(object sender, RoutedEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda zelite da odobrite \nmanifestaciju?", 0);
            wk.ShowDialog();

            if (wk.Result == MessageBoxResult.Yes)
            {
                using (var db = new DatabaseContext())
                {
                    Manifestacija toComplete = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).ToArray()[0];
                    toComplete.Status = StatusManifestacije.ZAVRSENA;
                    db.SaveChanges();
                }
                using (var db = new DatabaseContext())
                {
                    List<Manifestacija> manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Obrisana != true select m).ToList();
                    ParentData.ItemsSource = new ObservableCollection<Manifestacija>(manifestations);
                }
                this.Close();
            }
        }

    }
}