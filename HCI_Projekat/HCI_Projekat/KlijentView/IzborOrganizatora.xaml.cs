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
    /// Interaction logic for IzborOrganizatora.xaml
    /// </summary>
    public partial class IzborOrganizatora : Window, INotifyPropertyChanged
    {
        public List<Organizator> Organizatori { get; set; }

        private List<Organizator> _izabran;

        public List<Organizator> Izabran
        {
            get
            {
                return _izabran;
            }
            set
            {
                _izabran = value;
                OnPropertyChanged("Izabran");
            }
        }

        public IzborOrganizatora(List<Organizator> iz)
        {
            InitializeComponent();
            Izabran = iz;
            using (var db = new DatabaseContext())
            {
                Organizatori = (from org in db.Korisnici where org.Uloga == UlogaKorisnika.ORGANIZATOR select org as Organizator).ToList();
                foreach(Organizator o in Organizatori)
                {
                    o.Zauzetost = (from man in db.Manifestacije where man.Organizator.Id == o.Id && man.Status == StatusManifestacije.U_IZRADI select man).Count();
                }
            }

            organizatori.ItemsSource = new ObservableCollection<Organizator>(Organizatori);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void Izaberi_Click(object sender, RoutedEventArgs e)
        {
            Organizator selected = (Organizator)organizatori.SelectedItem;
            using (var db = new DatabaseContext())
            {
                Organizator o = (from org in db.Korisnici where org.Id == selected.Id select org).FirstOrDefault() as Organizator;
                Izabran.Clear();
                Izabran.Add(o);
            }
            this.Close();
        }
    }
}
