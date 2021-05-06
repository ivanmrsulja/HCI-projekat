using HCI_Projekat.Model;
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
    /// Interaction logic for KlijentHOME.xaml
    /// </summary>
    public partial class KlijentHOME : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICollectionView _View;
        private List<TemaManifestacije> _teme;
        public ICollectionView View
        {
            get
            {
                return _View;
            }
            set
            {
                _View = value;
                OnPropertyChanged("View");
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

        public Window ParentScreen { get; set; }
        public Korisnik Klijent { get; set; }

        public ObservableCollection<Manifestacija> Manifestacije { get; set; }


        public KlijentHOME(Window p, Korisnik k)
        {
            InitializeComponent();
            this.DataContext = this;
            ParentScreen = p;
            ParentScreen.Hide();
            Klijent = k;
            Teme = new List<TemaManifestacije> { TemaManifestacije.RODJENDAN, TemaManifestacije.KOKTEL_PARTY, TemaManifestacije.OTVARANJE, TemaManifestacije.REJV, TemaManifestacije.VENCANJE, TemaManifestacije.SVE };
            using (var db = new DatabaseContext())
            {
                List<Manifestacija> manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Obrisana != true select m).ToList();
                Manifestacije = new ObservableCollection<Manifestacija>(manifestations);
                View = CollectionViewSource.GetDefaultView(Manifestacije);
            }
        }

        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda zelite da se odjavite?", 0);
            wk.ShowDialog();

            if (wk.Result == MessageBoxResult.Yes)
            {
                ParentScreen.Show();
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void Odjava(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void DodajManifestaciju(object sender, RoutedEventArgs e)
        {
            var w = new DodajManifestaciju(Klijent as Klijent, dgrMain, Manifestacije);
            w.ShowDialog();
        }

        public void PretraziDatum(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(tema.SelectedItem == null);
            DateTime d = DateTime.Parse(datum.Text);
            using (var db = new DatabaseContext())
            {
                List<Manifestacija> manifestations = null;
                if (tema.SelectedItem == null)
                {
                    manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Obrisana != true select m).ToList();
                }
                else
                {
                    TemaManifestacije t = (TemaManifestacije)tema.SelectedItem;
                    if (t == TemaManifestacije.SVE)
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Obrisana != true select m).ToList();
                    }
                    else
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Tema == t && m.Obrisana != true select m).ToList();
                    }
                }
                Manifestacije = new ObservableCollection<Manifestacija>(manifestations);
                View = CollectionViewSource.GetDefaultView(Manifestacije);
                dgrMain.ItemsSource = null;
                dgrMain.ItemsSource = Manifestacije;
            }
        }

        public void Filtriraj(object sender, RoutedEventArgs e)
        {
            TemaManifestacije t = (TemaManifestacije) tema.SelectedItem;
            using (var db = new DatabaseContext())
            {
                List<Manifestacija> manifestations = null;
                if (t == TemaManifestacije.SVE)
                {
                    if (datum.Text == "")
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Obrisana != true select m).ToList();
                    }
                    else
                    {
                        DateTime d = DateTime.Parse(datum.Text);
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Obrisana != true select m).ToList();
                    }
                }
                else
                {
                    if (datum.Text == "")
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Tema == t && m.Obrisana != true select m).ToList();
                    }
                    else
                    {
                        DateTime d = DateTime.Parse(datum.Text);
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Tema == t && m.Obrisana != true select m).ToList();
                    }
                }
                Manifestacije = new ObservableCollection<Manifestacija>(manifestations);
                View = CollectionViewSource.GetDefaultView(Manifestacije);
                dgrMain.ItemsSource = null;
                dgrMain.ItemsSource = Manifestacije;
            }
        }

        private void Pomoc(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wk = new Profil(Klijent as Klijent);

            if ((bool)wk.ShowDialog() == true)
            {
                var value = wk.GetValue();
                Klijent = value as Klijent;
            }
        }

        private void Pregledaj_Click(object sender, EventArgs e)
        {
            var w = new PregledManifestacije((Manifestacija)dgrMain.SelectedItem, dgrMain, Klijent);
            w.ShowDialog();
        }
    }
}
