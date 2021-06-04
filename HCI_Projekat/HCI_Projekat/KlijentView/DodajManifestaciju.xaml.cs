using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DodajManifestaciju.xaml
    /// </summary>
    public partial class DodajManifestaciju : Window, INotifyPropertyChanged
    {
        public Klijent Klijent { get; set; }
        public DataGrid BindedGrid { get; set; }
        public ObservableCollection<Manifestacija> Manifestacije { get; set; }
        private List<TemaManifestacije> _teme;
        private string FileName;
        List<Gost> listGostiju = new List<Gost>();

        private double _budzet;
        private int _gosti;

        public List<Organizator> Organizator { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DodajManifestaciju(Klijent k, DataGrid data, ObservableCollection<Manifestacija> mans)
        {
            InitializeComponent();
            this.DataContext = this;
            Teme = new List<TemaManifestacije> { TemaManifestacije.RODJENDAN, TemaManifestacije.KOKTEL_PARTY, TemaManifestacije.OTVARANJE, TemaManifestacije.REJV, TemaManifestacije.VENCANJE};
            Klijent = k;
            BindedGrid = data;
            Manifestacije = mans;
            Organizator = new List<Organizator>();
        }

        private void FormStateChanged(object sender, RoutedEventArgs e)
        {
            int brGosti = 0;
            double budz = 0;
            if (brojGostiju.Text!="")
            {
                try
                {
                    brGosti = Int32.Parse(brojGostiju.Text);
                }
                catch
                {
                    potvrdi.IsEnabled = false;
                }
            }
            if (budzet.Text != "")
            {
                try
                {
                    budz = Double.Parse(budzet.Text);
                }
                catch
                {
                    potvrdi.IsEnabled = false;
                }
            }
            if (tema.Text.Trim() == "" || mesto.Text.Trim() == "" || muzika.Text.Trim() == "" || dekoracije.Text.Trim() == ""  || budz<1|| brGosti< 1 || datum.Text.Trim() == ""|| dodatniZahtevi.Text.Trim() == "")
                potvrdi.IsEnabled = false;
            else
            {
                potvrdi.IsEnabled = true;
            }
            if(datum.SelectedDate !=null)
            {
                if (datum.SelectedDate < DateTime.Now)
                {
                    datum.SelectedDate = null;
                }
            }

        }

        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public List<TemaManifestacije> Teme
        {
            get
            {
                return _teme;
            }
            set
            {
                _teme = value;
                OnPropertyChanged("Teme");
            }
        }

        public double Budzet
        {
            get
            {
                return _budzet;
            }
            set
            {
                _budzet = value;
                OnPropertyChanged("Budzet");
            }
        }

        public int Gosti
        {
            get
            {
                return _gosti;
            }
            set
            {
                _gosti = value;
                OnPropertyChanged("Gosti");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Potvrdi_Click(object sender, RoutedEventArgs e)
        {
            TemaManifestacije t = stringToManifestacija(tema.Text.ToUpper());
            bool c = (fiksan.IsChecked == true) ? true : false;
            var d = Convert.ToDateTime(datum.Text).ToString("MM/dd/yyyy");
            using (var db = new DatabaseContext())
            {               
                Manifestacija novi = new Manifestacija(t,Double.Parse(budzet.Text),c,Int32.Parse(brojGostiju.Text),mesto.Text,dekoracije.Text,muzika.Text,dodatniZahtevi.Text,Convert.ToDateTime(datum.Text), null, null);
                foreach (var item in listGostiju)
                {
                    novi.AddGost(item);
                }
                if(listGostiju.Count==0)
                {
                    for(int i=1;i<=Int32.Parse(brojGostiju.Text);i++)
                    {
                        Gost tmp = new Gost("gost"+i.ToString(), 0, null);
                        listGostiju.Add(tmp);
                    }
                }
                if(Organizator.Count > 0)
                {
                    int id = Organizator[0].Id;
                    Organizator org = (from o in db.Korisnici where o.Id == id select o).FirstOrDefault() as Organizator;
                    org.AddManifestacija(novi);
                    novi.Status = StatusManifestacije.U_IZRADI;
                }
                Klijent klijent = (from k in db.Korisnici where k.Id == Klijent.Id select k).ToArray()[0] as Klijent;
                klijent.AddManifestacija(novi);
                db.Manifestacije.Add(novi);
                db.SaveChanges();
                List<Manifestacija> manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Obrisana != true select m).ToList();
                Manifestacije = new ObservableCollection<Manifestacija>(manifestations);
            }
            BindedGrid.ItemsSource = null;
            BindedGrid.ItemsSource = Manifestacije;
            var dijalog5 = new OkForm("Uspešno ste kreirali\nmanifestaciju", "Manifestacija kreirana");
            dijalog5.ShowDialog();
            this.Close();
        }

        private TemaManifestacije stringToManifestacija(string manUpper)
        {
            if (manUpper == "VENCANJE")
                return TemaManifestacije.VENCANJE;
            else if (manUpper == "RODJENDAN")
                return TemaManifestacije.RODJENDAN;
            else if (manUpper == "KOKTEL_PARTY")
                return TemaManifestacije.KOKTEL_PARTY;
            else if (manUpper == "REJV")
                return TemaManifestacije.REJV;
            else if (manUpper == "OTVARANJE")
                return TemaManifestacije.OTVARANJE;
            return TemaManifestacije.SVE;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    FileName = openFileDialog.FileName;

                int counter = 0;
                using (var reader = new StreamReader(FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        counter++;
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        Gost tmp = new Gost(values[0], 0, null);
                        listGostiju.Add(tmp);
                    }
                }
                brojGostiju.Text = counter.ToString();
            }
            catch
            {
                brojGostiju.Text = "0";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelp("HelpKorisnikDodajManifestaciju", this);
        }

        private void Izaberi_Click(object sender, RoutedEventArgs e)
        {
            var w = new IzborOrganizatora(Organizator);
            w.ShowDialog();
            if(Organizator.Count > 0)
            {
                imeOrganizatora.Text = Organizator[0].Ime + " " + Organizator[0].Prezime;
            }
        }
    }
}
