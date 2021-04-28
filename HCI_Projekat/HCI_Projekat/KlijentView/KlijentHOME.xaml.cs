using HCI_Projekat.Model;
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
                List<Manifestacija> manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id select m).ToList();
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
            ParentScreen.Show();
        }

        public void Odjava(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Da li ste sigurni da zelite da se odjavite?", "Odjava", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Hide();
                    ParentScreen.Show();
                    break;
                case MessageBoxResult.No:
                    break;
            }
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
                    manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d select m).ToList();
                }
                else
                {
                    TemaManifestacije t = (TemaManifestacije)tema.SelectedItem;
                    if (t == TemaManifestacije.SVE)
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d select m).ToList();
                    }
                    else
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Tema == t select m).ToList();
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
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id select m).ToList();
                    }
                    else
                    {
                        DateTime d = DateTime.Parse(datum.Text);
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d select m).ToList();
                    }
                }
                else
                {
                    if (datum.Text == "")
                    {
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.Tema == t select m).ToList();
                    }
                    else
                    {
                        DateTime d = DateTime.Parse(datum.Text);
                        manifestations = (from m in db.Manifestacije where m.Klijent.Id == Klijent.Id && m.DatumOdrzavanja == d && m.Tema == t select m).ToList();
                    }
                }
                Manifestacije = new ObservableCollection<Manifestacija>(manifestations);
                View = CollectionViewSource.GetDefaultView(Manifestacije);
                dgrMain.ItemsSource = null;
                dgrMain.ItemsSource = Manifestacije;
            }
        }
    }
}
