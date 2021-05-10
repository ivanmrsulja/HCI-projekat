using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HCI_Projekat.Administrator
{
    /// <summary>
    /// Interaction logic for HomeAdmin.xaml
    /// </summary>
    public partial class HomeAdmin : Window
    {
        public Window ParentScreen { get; set; }

        public ObservableCollection<Manifestacija> Manifestacije { get; set; }
        public ObservableCollection<Korisnik> Korisnici { get; set; }
        public ObservableCollection<Saradnik> Saradnici { get; set; }
        public ObservableCollection<Komentar> Komentari { get; set; }
        
        public HomeAdmin(Window p)
        {
            InitializeComponent();
            ParentScreen = p;
            ParentScreen.Hide();
            DataContext = this;

            using (var db = new DatabaseContext())
            {
                List<Manifestacija> manifestacije = (from man in db.Manifestacije where man.Obrisana == false select man).ToList();
                List<Korisnik> korisnici = (from kor in db.Korisnici where kor.Obrisan == false select kor).ToList();
                List<Saradnik> saradnici = (from sar in db.Saradnici where sar.Obrisan == false select sar).ToList();
                List<Komentar> komentari = (from kom in db.Komentari where kom.Obrisan == false select kom).ToList();
                Manifestacije = new ObservableCollection<Manifestacija>(manifestacije);
                Korisnici = new ObservableCollection<Korisnik>(korisnici);
                Saradnici = new ObservableCollection<Saradnik>(saradnici);
                Komentari = new ObservableCollection<Komentar>(komentari);
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

        public void Manifestacije_Click(object sender, RoutedEventArgs e)
        {
            KomLabel.Visibility = Visibility.Hidden;
            ManLabel.Visibility = Visibility.Visible;
            KorLabel.Visibility = Visibility.Hidden;
            SarLabel.Visibility = Visibility.Hidden;

            KomGrid.Visibility = Visibility.Hidden;
            ManGrid.Visibility = Visibility.Visible;
            KorGrid.Visibility = Visibility.Hidden;
            SarGrid.Visibility = Visibility.Hidden;

            DodajOrgBtn.Visibility = Visibility.Hidden;
            DodajSarBtn.Visibility = Visibility.Hidden;
            DodajOrgLabel.Visibility = Visibility.Hidden;
            DodajSarLabel.Visibility = Visibility.Hidden;

        }

        public void Korisnici_Click(object sender, RoutedEventArgs e)
        {
            KomLabel.Visibility = Visibility.Hidden;
            ManLabel.Visibility = Visibility.Hidden;
            KorLabel.Visibility = Visibility.Visible;
            SarLabel.Visibility = Visibility.Hidden;

            KomGrid.Visibility = Visibility.Hidden;
            ManGrid.Visibility = Visibility.Hidden;
            KorGrid.Visibility = Visibility.Visible;
            SarGrid.Visibility = Visibility.Hidden;

            DodajOrgBtn.Visibility = Visibility.Visible;
            DodajSarBtn.Visibility = Visibility.Hidden;
            DodajOrgLabel.Visibility = Visibility.Visible;
            DodajSarLabel.Visibility = Visibility.Hidden;
        }

        public void Saradnici_Click(object sender, RoutedEventArgs e)
        {
            KomLabel.Visibility = Visibility.Hidden;
            ManLabel.Visibility = Visibility.Hidden;
            KorLabel.Visibility = Visibility.Hidden;
            SarLabel.Visibility = Visibility.Visible;

            KomGrid.Visibility = Visibility.Hidden;
            ManGrid.Visibility = Visibility.Hidden;
            KorGrid.Visibility = Visibility.Hidden;
            SarGrid.Visibility = Visibility.Visible;

            DodajOrgBtn.Visibility = Visibility.Hidden;
            DodajSarBtn.Visibility = Visibility.Visible;
            DodajOrgLabel.Visibility = Visibility.Hidden;
            DodajSarLabel.Visibility = Visibility.Visible;
        }

        public void Komentari_Click(object sender, RoutedEventArgs e)
        {
            KomLabel.Visibility = Visibility.Visible;
            ManLabel.Visibility = Visibility.Hidden;
            KorLabel.Visibility = Visibility.Hidden;
            SarLabel.Visibility = Visibility.Hidden;

            KomGrid.Visibility = Visibility.Visible;
            ManGrid.Visibility = Visibility.Hidden;
            KorGrid.Visibility = Visibility.Hidden;
            SarGrid.Visibility = Visibility.Hidden;

            DodajOrgBtn.Visibility = Visibility.Hidden;
            DodajSarBtn.Visibility = Visibility.Hidden;
            DodajOrgLabel.Visibility = Visibility.Hidden;
            DodajSarLabel.Visibility = Visibility.Hidden;
        }

        public void Pomoc_Click(object sender, RoutedEventArgs e)
        {

        }

        public void DodajOrganizatora_Click(object sender, RoutedEventArgs e)
        {
            var w = new DodavanjeOrganizatora(null, KorGrid);
            w.ShowDialog();
        }

        public void DodajSaradnika_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ObrisiKomentar_Click(object sender, RoutedEventArgs e)
        {
            Komentar selected = (Komentar)KomGrid.SelectedItem;

            using (var db = new DatabaseContext())
            {
                var komentar = (from k in db.Komentari where k.Id == selected.Id select k).FirstOrDefault();
                komentar.Obrisan = true;

                db.SaveChanges();

                KomGrid.ItemsSource = new ObservableCollection<Komentar>((from kom in db.Komentari.Include("Manifestacija.Organizator").Include("Klijent") where kom.Obrisan == false select kom).ToList());
            }
        }

        public void Odjava_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RowKorisnik_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            if (KorGrid.SelectedItem.GetType() == typeof(Organizator))
            {
                Console.WriteLine((Korisnik)KorGrid.SelectedItem);
                var w = new DodavanjeOrganizatora((Organizator)KorGrid.SelectedItem, KorGrid);
                w.ShowDialog();
            }
        }

    }
}
