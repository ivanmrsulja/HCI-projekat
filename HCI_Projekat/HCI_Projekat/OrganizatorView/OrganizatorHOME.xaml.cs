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
                List<Manifestacija> nedodeljeno = (from m in db.Manifestacije.Include("Klijent") where CurrentUser.Id == m.Organizator.Id && m.Status == StatusManifestacije.NOVA && m.Obrisana != true select m).ToList();

                StareManifestacije = new ObservableCollection<Manifestacija>(istorija);
                AktuelneManifestacije = new ObservableCollection<Manifestacija>(aktuelno);
                NedodeljeneManifestacije = new ObservableCollection<Manifestacija>(nedodeljeno);
            }
            foreach (Manifestacija man in StareManifestacije)
            {
                Console.WriteLine(man.Klijent.Ime);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda zelite da se odjavite?", 0, "Odjava");
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
    }
}
