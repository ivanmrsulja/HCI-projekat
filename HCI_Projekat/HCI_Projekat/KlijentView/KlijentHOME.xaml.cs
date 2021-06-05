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
        private bool _undoOngoing;

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

        public ObservableCollection<Notifikacija> Notifikacije
        {
            get
            {
                return _notifikacije;
            }
            set
            {
                _notifikacije = value;
                OnPropertyChanged("Notifikacije");
            }
        }

        public Window ParentScreen { get; set; }
        public Korisnik Klijent { get; set; }

        public ObservableCollection<Manifestacija> Manifestacije { get; set; }
        public ObservableCollection<Notifikacija> _notifikacije { get; set; }

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
                Notifikacije = new ObservableCollection<Notifikacija>((from not in db.Notifikacije where not.Dismissed == false && not.Klijent.Id == Klijent.Id select not));
            }
            undoBtn.Visibility = Visibility.Collapsed;
            _undoOngoing = false;

            if (Manifestacije.Count == 0)
            {
                dgrMain.Visibility = Visibility.Hidden;
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
            var wk = new YesNo("Da li ste sigurni \nda želite da se odjavite?", 0, "Odjava");
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

        public void Undo_Click(object sender, RoutedEventArgs e)
        {
            _undoOngoing = true;
            datum.Text = "";
            if (tema.SelectedItem == null)
            {
                tema.SelectedItem = TemaManifestacije.SVE;
                tema.SelectedItem = null;
            }
            else
            {
                Filtriraj(sender, e);
            }
            undoBtn.Visibility = Visibility.Collapsed;
            _undoOngoing = false;
        }

        public void DodajManifestaciju(object sender, RoutedEventArgs e)
        {
            var w = new DodajManifestaciju(Klijent as Klijent, dgrMain, Manifestacije);
            w.ShowDialog();
            if(dgrMain.Items.Count > 0)
            {
                dgrMain.Visibility = Visibility.Visible;
            }
        }

        public void Odbaci_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Notifikacija current = button.DataContext as Notifikacija;
            if (current == null) { return; }
            using(var db = new DatabaseContext())
            {
                Notifikacija toDismiss = (from n in db.Notifikacije where n.Id == current.Id select n).FirstOrDefault();
                toDismiss.Dismissed = true;
                db.SaveChanges();
                Notifikacije = new ObservableCollection<Notifikacija>((from not in db.Notifikacije where not.Dismissed == false && not.Klijent.Id == Klijent.Id select not));
            }
        }

        public void PretraziDatum(object sender, RoutedEventArgs e)
        {
            if (datum.Text == "")
            {
                return;
            }
            if (_undoOngoing)
            {
                return;
            }
            undoBtn.Visibility = Visibility.Visible;
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
            if (tema.SelectedItem == null)
            {
                return;
            }
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
            Manifestacija selected = (Manifestacija)dgrMain.SelectedItem;
            var w = new PregledManifestacije(selected, dgrMain, Klijent);
            w.ShowDialog();
        }

        private void Notification_DoubleClick(object sender, EventArgs e)
        {
            Notifikacija selected = (Notifikacija)notificationList.SelectedItem;
            Manifestacija man;
            using (var db = new DatabaseContext())
            {
                man = (from m in db.Manifestacije where m.Id == selected.IDManifestacije select m).FirstOrDefault();
            }
            var w = new PregledManifestacije(man, dgrMain, Klijent);
            w.ShowDialog();
        }

        private void Pomoc(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelp("HelpKorisnikHome", this);
        }
    }
}
