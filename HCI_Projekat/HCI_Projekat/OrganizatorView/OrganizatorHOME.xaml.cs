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

namespace HCI_Projekat.OrganizatorView
{
    /// <summary>
    /// Interaction logic for OrganizatorHOME.xaml
    /// </summary>
    public partial class OrganizatorHOME : Window, INotifyPropertyChanged
    {
        public Window ParentScreen { get; set; }
       
        private List<TemaManifestacije> _teme;

        public ObservableCollection<Manifestacija> StareManifestacije { get; set; }
        
        public ObservableCollection<Manifestacija> AktuelneManifestacije { get; set; }

        public ObservableCollection<Manifestacija> NedodeljeneManifestacije { get; set; }

        public ObservableCollection<Saradnik> _saradnici { get; set; }

        public Korisnik CurrentUser { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
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

        public ObservableCollection<Saradnik> Saradnici
        {
            get
            {
                return _saradnici;
            }
            set
            {
                _saradnici = value;
                OnPropertyChanged("Saradnici");
            }
        }

        public OrganizatorHOME(Window p, Korisnik current)
        {
            InitializeComponent();
            ParentScreen = p;
            CurrentUser = current;
            ParentScreen.Hide();
            DataContext = this;

            Teme = new List<TemaManifestacije> { TemaManifestacije.RODJENDAN, TemaManifestacije.KOKTEL_PARTY, TemaManifestacije.OTVARANJE, TemaManifestacije.REJV, TemaManifestacije.VENCANJE, TemaManifestacije.SVE };
            using (var db = new DatabaseContext())
            {
                List<Manifestacija> istorija = (from m in db.Manifestacije.Include("Klijent") where CurrentUser.Id == m.Organizator.Id && m.Status == StatusManifestacije.ZAVRSENA && m.Obrisana != true select m).ToList();
                List<Manifestacija> aktuelno = (from m in db.Manifestacije.Include("Klijent") where CurrentUser.Id == m.Organizator.Id && m.Status == StatusManifestacije.U_IZRADI && m.Obrisana != true select m).ToList();
                List<Manifestacija> nedodeljeno = (from m in db.Manifestacije.Include("Klijent") where m.Status == StatusManifestacije.NOVA && m.Obrisana != true select m).ToList();

                StareManifestacije = new ObservableCollection<Manifestacija>(istorija);
                AktuelneManifestacije = new ObservableCollection<Manifestacija>(aktuelno);
                NedodeljeneManifestacije = new ObservableCollection<Manifestacija>(nedodeljeno);
                Saradnici = new ObservableCollection<Saradnik>(from sar in db.Saradnici where sar.Obrisan == false select sar);
            }

            undoBtn.Visibility = Visibility.Collapsed;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda želite da se odjavite?", 0, "Odjava", true);
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

        public void Undo_Click(object sender, RoutedEventArgs e)
        {
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
        }

        public void Istorija_Click(object sender, EventArgs e)
        {
            istorija.Visibility = Visibility.Visible;
            aktuelno.Visibility = Visibility.Hidden;
            nedodeljeno.Visibility = Visibility.Hidden;
            saradnici.Visibility = Visibility.Hidden;

            searchBar.Visibility = Visibility.Visible;
            aktuelnoLabel.Visibility = Visibility.Hidden;
            nedodeljenoLabel.Visibility = Visibility.Hidden;
            saradniciLabel.Visibility = Visibility.Hidden;
            DodajSarLabel.Visibility = Visibility.Hidden;
            DodajSarBtn.Visibility = Visibility.Hidden;
        }

        public void Aktuelno_Click(object sender, EventArgs e)
        {
            istorija.Visibility = Visibility.Hidden;
            aktuelno.Visibility = Visibility.Visible;
            nedodeljeno.Visibility = Visibility.Hidden;
            saradnici.Visibility = Visibility.Hidden;

            searchBar.Visibility = Visibility.Hidden;
            aktuelnoLabel.Visibility = Visibility.Visible;
            nedodeljenoLabel.Visibility = Visibility.Hidden;
            saradniciLabel.Visibility = Visibility.Hidden;
            DodajSarLabel.Visibility = Visibility.Hidden;
            DodajSarBtn.Visibility = Visibility.Hidden;
        }

        public void Preuzmi_Click(object sender, EventArgs e)
        {
            Manifestacija selected = (Manifestacija)nedodeljeno.SelectedItem;

            var wk = new YesNo("Da li ste sigurni \nda želite da preuzmete\norganizaciju manifestacije\nkorisnika " + selected.Klijent.Ime + " " + selected.Klijent.Prezime + "?", 0, "Preuzmi manifestaciju", true);
            wk.ShowDialog();

            if (wk.Result == MessageBoxResult.Yes)
            {
                using (var db = new DatabaseContext())
                {
                    var manifestacija = (from m in db.Manifestacije.Include("Klijent") where m.Id == selected.Id select m).FirstOrDefault();
                    var organizator = (from o in db.Korisnici where o.Id == CurrentUser.Id select o).FirstOrDefault() as Organizator;

                    Console.WriteLine(manifestacija.Id);

                    organizator.AddManifestacija(manifestacija);
                    manifestacija.Status = StatusManifestacije.U_IZRADI;

                    db.SaveChanges();
                    aktuelno.ItemsSource = new ObservableCollection<Manifestacija>((from m in db.Manifestacije.Include("Klijent") where CurrentUser.Id == m.Organizator.Id && m.Status == StatusManifestacije.U_IZRADI && m.Obrisana != true select m).ToList());
                    nedodeljeno.ItemsSource = new ObservableCollection<Manifestacija>((from m in db.Manifestacije.Include("Klijent") where m.Status == StatusManifestacije.NOVA && m.Obrisana != true select m).ToList());

                    var ok = new OkForm("Uspešno preuzeto.\nManifestacija se nalazi u\nsekciji 'Aktuelno'.", "Uspešno preuzeto", true);
                    ok.ShowDialog();
                }
            }
        }

        public void Nedodeljeno_Click(object sender, EventArgs e)
        {
            istorija.Visibility = Visibility.Hidden;
            aktuelno.Visibility = Visibility.Hidden;
            nedodeljeno.Visibility = Visibility.Visible;
            saradnici.Visibility = Visibility.Hidden;

            searchBar.Visibility = Visibility.Hidden;
            aktuelnoLabel.Visibility = Visibility.Hidden;
            nedodeljenoLabel.Visibility = Visibility.Visible;
            saradniciLabel.Visibility = Visibility.Hidden;
            DodajSarLabel.Visibility = Visibility.Hidden;
            DodajSarBtn.Visibility = Visibility.Hidden;
        }

        public void Saradnici_Click(object sender, EventArgs e)
        {
            istorija.Visibility = Visibility.Hidden;
            aktuelno.Visibility = Visibility.Hidden;
            nedodeljeno.Visibility = Visibility.Hidden;
            saradnici.Visibility = Visibility.Visible;

            searchBar.Visibility = Visibility.Hidden;
            aktuelnoLabel.Visibility = Visibility.Hidden;
            nedodeljenoLabel.Visibility = Visibility.Hidden;
            saradniciLabel.Visibility = Visibility.Visible;
            DodajSarLabel.Visibility = Visibility.Visible;
            DodajSarBtn.Visibility = Visibility.Visible;
        }

        public void ObrisiSaradnika_Click(object sender, EventArgs e)
        {
            Saradnik current = (Saradnik)saradnici.SelectedItem;
            var wk = new YesNo("Da li ste sigurni da\nželite da obrišete saradnika\n" + current.Naziv + "?", 0, "Potvrda brisanja", true);
            wk.ShowDialog();
            if(wk.Result != MessageBoxResult.Yes)
            {
                return;
            }
            using (var db = new DatabaseContext())
            {
                Saradnik toDelete = (from sar in db.Saradnici where sar.Id == current.Id select sar).FirstOrDefault();
                toDelete.Obrisan = true;
                foreach(Ponuda p in toDelete.Ponude)
                {
                    for (int i = p.Manifestacije.Count - 1; i >= 0; i--)
                    {
                        if (p.Stolovi.Count > 0 && p.Manifestacije[i].Status != StatusManifestacije.ZAVRSENA)
                        {
                            foreach (Gost g in p.Manifestacije[i].Gosti)
                            {
                                g.BrojStola = 0;
                            }
                        }
                        if(p.Manifestacije[i].Status != StatusManifestacije.ZAVRSENA)
                        {
                            p.Manifestacije[i].PredlozenoZaZavrsavanje = false; // da ne potvrdi a u medjuvremenu je neko izbrisao nesto
                            p.Manifestacije[i].RemovePonuda(p);
                        }
                    }
                }
                db.SaveChanges();
                Saradnici = new ObservableCollection<Saradnik>(from sar in db.Saradnici where sar.Obrisan == false select sar);
                saradnici.ItemsSource = Saradnici;
            }
        }

        public void IzmeniSaradnika_Click(object sender, EventArgs e)
        {
            Saradnik current = (Saradnik)saradnici.SelectedItem;
            var w = new AzurirajSaradnika(current, true);
            w.ShowDialog();
            using (var db = new DatabaseContext())
            {
                Saradnici = new ObservableCollection<Saradnik>(from sar in db.Saradnici where sar.Obrisan == false select sar);
                saradnici.ItemsSource = Saradnici;
            }

        }
        public void Detaljnije_Click(object sender, EventArgs e)
        {
            Manifestacija current = (Manifestacija)nedodeljeno.SelectedItem;
            var w = new PregledNedodeljeneManifestacije(current);
            w.ShowDialog();
        }

        public void DodajSaradnika_Click(object sender, EventArgs e)
        {
            var w = new DodajSaradnika(true);
            w.ShowDialog();
            using (var db = new DatabaseContext())
            {
                Saradnici = new ObservableCollection<Saradnik>((from sar in db.Saradnici where sar.Obrisan == false select sar));
                saradnici.ItemsSource = Saradnici;
            }
        }

        public void Odjava(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void PretraziDatum(object sender, RoutedEventArgs e)
        {
            if (datum.Text == "")
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
                    manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.DatumOdrzavanja == d && m.Status == StatusManifestacije.ZAVRSENA && m.Obrisana != true select m).ToList();
                }
                else
                {
                    TemaManifestacije t = (TemaManifestacije)tema.SelectedItem;
                    if (t == TemaManifestacije.SVE)
                    {
                        manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.DatumOdrzavanja == d && m.Status == StatusManifestacije.ZAVRSENA && m.Obrisana != true select m).ToList();
                    }
                    else
                    {
                        manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.DatumOdrzavanja == d && m.Status == StatusManifestacije.ZAVRSENA && m.Tema == t && m.Obrisana != true select m).ToList();
                    }
                }
                istorija.ItemsSource = new ObservableCollection<Manifestacija>(manifestations);
            }
        }

        public void Filtriraj(object sender, RoutedEventArgs e)
        {
            if (tema.SelectedItem == null)
            {
                return;
            }
            TemaManifestacije t = (TemaManifestacije)tema.SelectedItem;
            using (var db = new DatabaseContext())
            {
                List<Manifestacija> manifestations = null;
                if (t == TemaManifestacije.SVE)
                {
                    if (datum.Text == "")
                    {
                        manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.Obrisana != true && m.Status == StatusManifestacije.ZAVRSENA select m).ToList();
                    }
                    else
                    {
                        DateTime d = DateTime.Parse(datum.Text);
                        manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.DatumOdrzavanja == d && m.Obrisana != true && m.Status == StatusManifestacije.ZAVRSENA select m).ToList();
                    }
                }
                else
                {
                    if (datum.Text == "")
                    {
                        manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.Tema == t && m.Obrisana != true && m.Status == StatusManifestacije.ZAVRSENA select m).ToList();
                    }
                    else
                    {
                        DateTime d = DateTime.Parse(datum.Text);
                        manifestations = (from m in db.Manifestacije.Include("Klijent") where m.Organizator.Id == CurrentUser.Id && m.DatumOdrzavanja == d && m.Tema == t && m.Status == StatusManifestacije.ZAVRSENA && m.Obrisana != true select m).ToList();
                    }
                }
                istorija.ItemsSource = new ObservableCollection<Manifestacija>(manifestations);
            }
        }

        public void DetaljnijeIstorija_Click(object sender, EventArgs e)
        {
            Manifestacija selected = (Manifestacija)istorija.SelectedItem;
            var w = new PregledStareManifestacije(selected);
            w.ShowDialog();
        }

        public void DetaljnijeAktuelno_Click(object sender, EventArgs e)
        {
            Manifestacija selected = (Manifestacija)aktuelno.SelectedItem;
            var w = new PregledAktuelneManifestacije(selected);
            w.ShowDialog();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DependencyObject)
            {
                string str = HelpProvider.GetHelpKey((DependencyObject)sender);
                HelpProvider.ShowHelp(str, this, true);
            }
        }

        public void Pomoc_Click(object sender, EventArgs e)
        {
            HelpProvider.ShowHelp("HelpOrganizatorHome", this, true);
        }

    }
}
