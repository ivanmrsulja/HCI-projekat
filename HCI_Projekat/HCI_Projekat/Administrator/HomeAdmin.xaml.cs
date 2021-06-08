using HCI_Projekat.Model;
using HCI_Projekat.OrganizatorView;
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

namespace HCI_Projekat.Administrator
{
    /// <summary>
    /// Interaction logic for HomeAdmin.xaml
    /// </summary>
    public partial class HomeAdmin : Window, INotifyPropertyChanged
    {
        public Window ParentScreen { get; set; }

        public ObservableCollection<Manifestacija> Manifestacije { get; set; }
        public ObservableCollection<Korisnik> Korisnici { get; set; }
        public ObservableCollection<Saradnik> Saradnici { get; set; }
        public ObservableCollection<Komentar> Komentari { get; set; }

        public static RoutedCommand NewCommand = new RoutedCommand();
        public static RoutedCommand QCommand = new RoutedCommand();
        public static RoutedCommand WCommand = new RoutedCommand();
        public static RoutedCommand ECommand = new RoutedCommand();
        public static RoutedCommand RCommand = new RoutedCommand();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public int CurrentPageNum { get; set; }
        public int PageSize { get; set; }
        
        private int _maxPageSize;

        public int MaxPageSize
        {
            get
            {
                return _maxPageSize;
            }
            set
            {
                if (value != _maxPageSize)
                {
                    _maxPageSize = value;
                    OnPropertyChanged("MaxPageSize");
                }
            }
        }

        public HomeAdmin(Window p)
        {
            InitializeComponent();
            ParentScreen = p;
            ParentScreen.Hide();
            DataContext = this;

            CurrentPageNum = 1;
            PageSize = 5; // moze da se narihtava

            NewCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            QCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
            WCommand.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
            ECommand.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            RCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));

            using (var db = new DatabaseContext())
            {
                int itemCount = (from man in db.Manifestacije where man.Obrisana == false select man).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    ManGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }

                if (CurrentPageNum == 1)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = true;
                }
                else
                {
                    btnPrev.IsEnabled = true;
                    btnNext.IsEnabled = true;
                }

                if (CurrentPageNum == MaxPageSize)
                {
                    btnNext.IsEnabled = false;
                }

                List<Manifestacija> manifestacije = (from man in db.Manifestacije.Include("Organizator").Include("Klijent") where man.Obrisana == false orderby man.Id select man).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                //List<Korisnik> korisnici = (from kor in db.Korisnici where kor.Obrisan == false orderby kor.Id select kor).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                //List<Saradnik> saradnici = (from sar in db.Saradnici where sar.Obrisan == false orderby sar.Id select sar).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                //List<Komentar> komentari = (from kom in db.Komentari.Include("Manifestacija.Organizator").Include("Klijent") where kom.Obrisan == false orderby kom.Id select kom).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                Manifestacije = new ObservableCollection<Manifestacija>(manifestacije);
                //Korisnici = new ObservableCollection<Korisnik>(korisnici);
                //Saradnici = new ObservableCollection<Saradnik>(saradnici);
                //Komentari = new ObservableCollection<Komentar>(komentari);
            }
        }

        private void OnNextClicked(object sender, RoutedEventArgs e)
        {
            CurrentPageNum++;
            using(var db = new DatabaseContext())
            {
                if(ManGrid.Visibility == Visibility.Visible)
                {
                    List<Manifestacija> manifestacije = (from man in db.Manifestacije.Include("Organizator").Include("Klijent") where man.Obrisana == false orderby man.Id select man).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Manifestacije = new ObservableCollection<Manifestacija>(manifestacije);
                    ManGrid.ItemsSource = Manifestacije;
                }
                else if (KorGrid.Visibility == Visibility.Visible)
                {
                    List<Korisnik> korisnici = (from kor in db.Korisnici where kor.Obrisan == false orderby kor.Id select kor).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Korisnici = new ObservableCollection<Korisnik>(korisnici);
                    KorGrid.ItemsSource = Korisnici;
                }
                else if (SarGrid.Visibility == Visibility.Visible)
                {
                    List<Saradnik> saradnici = (from sar in db.Saradnici where sar.Obrisan == false orderby sar.Id select sar).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Saradnici = new ObservableCollection<Saradnik>(saradnici);
                    SarGrid.ItemsSource = Saradnici;
                }
                else if (KomGrid.Visibility == Visibility.Visible)
                {
                    List<Komentar> komentari = (from kom in db.Komentari.Include("Manifestacija.Organizator").Include("Klijent") where kom.Obrisan == false orderby kom.Id select kom).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Komentari = new ObservableCollection<Komentar>(komentari);
                    KomGrid.ItemsSource = Komentari;
                }
                
            }

            PageNum.Text = CurrentPageNum + "";
            if (CurrentPageNum == MaxPageSize)
            {
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = false;
            }
            else
            {
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
            }

            if (CurrentPageNum == 1)
            {
                btnPrev.IsEnabled = false;
            }
        }

        private void OnPreviousClicked(object sender, RoutedEventArgs e)
        {
            CurrentPageNum--;
            using (var db = new DatabaseContext())
            {
                if (ManGrid.Visibility == Visibility.Visible)
                {
                    List<Manifestacija> manifestacije = (from man in db.Manifestacije.Include("Organizator").Include("Klijent") where man.Obrisana == false orderby man.Id select man).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Manifestacije = new ObservableCollection<Manifestacija>(manifestacije);
                    ManGrid.ItemsSource = Manifestacije;
                }
                else if (KorGrid.Visibility == Visibility.Visible)
                {
                    List<Korisnik> korisnici = (from kor in db.Korisnici where kor.Obrisan == false orderby kor.Id select kor).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Korisnici = new ObservableCollection<Korisnik>(korisnici);
                    KorGrid.ItemsSource = Korisnici;
                }
                else if (SarGrid.Visibility == Visibility.Visible)
                {
                    List<Saradnik> saradnici = (from sar in db.Saradnici where sar.Obrisan == false orderby sar.Id select sar).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Saradnici = new ObservableCollection<Saradnik>(saradnici);
                    SarGrid.ItemsSource = Saradnici;
                }
                else if (KomGrid.Visibility == Visibility.Visible)
                {
                    List<Komentar> komentari = (from kom in db.Komentari.Include("Manifestacija.Organizator").Include("Klijent") where kom.Obrisan == false orderby kom.Id select kom).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                    Komentari = new ObservableCollection<Komentar>(komentari);
                    KomGrid.ItemsSource = Komentari;
                }
            }

            PageNum.Text = CurrentPageNum + "";
            if (CurrentPageNum == 1)
            {
                btnPrev.IsEnabled = false;
                btnNext.IsEnabled = true;
            }
            else
            {
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
            }

            if (CurrentPageNum == MaxPageSize)
            {
                btnNext.IsEnabled = false;
            }
        }

        private void OnSelectedPage(object sender, RoutedEventArgs e)
        {
            int page;
            try
            {
                page = Int32.Parse(PageNum.Text);
                if (page < 1 || page > MaxPageSize)
                {
                    return;
                }
                CurrentPageNum = page - 1;
                OnNextClicked(sender, e);
            }
            catch
            {
                //PageNum.Text = CurrentPageNum + "";
                return;
            }
        }

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(SarGrid.Visibility == Visibility.Visible)
            {
                DodajSaradnika_Click(sender, e);
            }
            else if (KorGrid.Visibility == Visibility.Visible)
            {
                DodajOrganizatora_Click(sender, e);
            }
        }

        private void QCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Manifestacije_Click(sender, e);
        }

        private void WCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Korisnici_Click(sender, e);
        }

        private void ECommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Saradnici_Click(sender, e);
        }

        private void RCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Komentari_Click(sender, e);
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

            CurrentPageNum = 1;

            using (var db = new DatabaseContext())
            {
                int itemCount = (from man in db.Manifestacije where man.Obrisana == false select man).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    ManGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }
                List<Manifestacija> manifestacije = (from man in db.Manifestacije.Include("Organizator").Include("Klijent") where man.Obrisana == false orderby man.Id select man).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                Manifestacije = new ObservableCollection<Manifestacija>(manifestacije);
                ManGrid.ItemsSource = Manifestacije;

                PageNum.Text = CurrentPageNum + "";
                if (CurrentPageNum == 1)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = true;
                }
                else
                {
                    btnPrev.IsEnabled = true;
                    btnNext.IsEnabled = true;
                }

                if (CurrentPageNum == MaxPageSize)
                {
                    btnNext.IsEnabled = false;
                }
            }

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

            CurrentPageNum = 1;

            using (var db = new DatabaseContext())
            {
                int itemCount = (from kor in db.Korisnici where kor.Obrisan == false select kor).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    KorGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }
                List<Korisnik> korisnici = (from kor in db.Korisnici where kor.Obrisan == false orderby kor.Id select kor).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                Korisnici = new ObservableCollection<Korisnik>(korisnici);
                KorGrid.ItemsSource = Korisnici;

                PageNum.Text = CurrentPageNum + "";
                if (CurrentPageNum == 1)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = true;
                }
                else
                {
                    btnPrev.IsEnabled = true;
                    btnNext.IsEnabled = true;
                }

                if (CurrentPageNum == MaxPageSize)
                {
                    btnNext.IsEnabled = false;
                }
            }

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

            CurrentPageNum = 1;

            using (var db = new DatabaseContext())
            {
                int itemCount = (from sar in db.Saradnici where sar.Obrisan == false select sar).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    SarGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }

                List<Saradnik> saradnici = (from sar in db.Saradnici where sar.Obrisan == false orderby sar.Id select sar).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                Saradnici = new ObservableCollection<Saradnik>(saradnici);
                SarGrid.ItemsSource = Saradnici;

                PageNum.Text = CurrentPageNum + "";
                if (CurrentPageNum == 1)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = true;
                }
                else
                {
                    btnPrev.IsEnabled = true;
                    btnNext.IsEnabled = true;
                }

                if (CurrentPageNum == MaxPageSize)
                {
                    btnNext.IsEnabled = false;
                }
            }
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

            CurrentPageNum = 1;

            using (var db = new DatabaseContext())
            {
                int itemCount = (from kom in db.Komentari where kom.Obrisan == false select kom).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    KomGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }

                List<Komentar> komentari = (from kom in db.Komentari.Include("Manifestacija.Organizator").Include("Klijent") where kom.Obrisan == false orderby kom.Id select kom).Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
                Komentari = new ObservableCollection<Komentar>(komentari);
                KomGrid.ItemsSource = Komentari;

                PageNum.Text = CurrentPageNum + "";
                if (CurrentPageNum == 1)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = true;
                }
                else
                {
                    btnPrev.IsEnabled = true;
                    btnNext.IsEnabled = true;
                }

                if (CurrentPageNum == MaxPageSize)
                {
                    btnNext.IsEnabled = false;
                }
            }
        }

        public void Pomoc_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelp("HelpAdminHome", this);
        }

        public void DodajOrganizatora_Click(object sender, RoutedEventArgs e)
        {
            var w = new DodavanjeOrganizatora(null, KorGrid);
            w.ShowDialog();
            using (var db = new DatabaseContext())
            {
                int itemCount = (from kor in db.Korisnici where kor.Obrisan == false select kor).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    KorGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }
            }
            CurrentPageNum--;
            OnNextClicked(sender, e);
        }

        public void DodajSaradnika_Click(object sender, RoutedEventArgs e)
        {
            var w = new DodajSaradnika();
            w.ShowDialog();
            using (var db = new DatabaseContext())
            {
                int itemCount = (from sar in db.Saradnici where sar.Obrisan == false select sar).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    SarGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }
            }
            CurrentPageNum--;
            OnNextClicked(sender, e);
        }

        public void ObrisiSaradnika_Click(object sender, RoutedEventArgs e)
        {
            Saradnik selected = (Saradnik)SarGrid.SelectedItem;

            var wk = new YesNo("Da li ste sigurni da\nželite da obrišete saradnika\n" + selected.Naziv + "?", 0, "Potvrda brisanja");
            wk.ShowDialog();
            if (wk.Result != MessageBoxResult.Yes)
            {
                return;
            }

            using (var db = new DatabaseContext())
            {
                Saradnik toDelete = (from sar in db.Saradnici where sar.Id == selected.Id select sar).FirstOrDefault();
                toDelete.Obrisan = true;
                //db.SaveChanges();
                //Saradnici = new ObservableCollection<Saradnik>((from sar in db.Saradnici where sar.Obrisan == false select sar));
                //SarGrid.ItemsSource = Saradnici;
                //var toUpdate = from man in db.Manifestacije where man.Obrisana == false && man.Status == StatusManifestacije.U_IZRADI select man;
                //foreach(var manifestacija in toUpdate)
                //{
                //    manifestacija.Ponude.RemoveAll(item => item.Saradnik.Id == toDelete.Id);
                //}
                foreach (Ponuda p in toDelete.Ponude)
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
                        if (p.Manifestacije[i].Status != StatusManifestacije.ZAVRSENA)
                        {
                            p.Manifestacije[i].PredlozenoZaZavrsavanje = false; // da ne potvrdi a u medjuvremenu je neko izbrisao nesto
                            p.Manifestacije[i].RemovePonuda(p);
                        }
                    }
                }
                db.SaveChanges();

                int itemCount = (from sar in db.Saradnici where sar.Obrisan == false select sar).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    SarGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }

                if (CurrentPageNum > MaxPageSize)
                {
                    OnPreviousClicked(sender, e);
                }
                else
                {
                    CurrentPageNum--;
                    OnNextClicked(sender, e);
                }

            }
        }

        public void ObrisiKomentar_Click(object sender, RoutedEventArgs e)
        {
            Komentar selected = (Komentar)KomGrid.SelectedItem;

            using (var db = new DatabaseContext())
            {
                var komentar = (from k in db.Komentari where k.Id == selected.Id select k).FirstOrDefault();
                komentar.Obrisan = true;

                db.SaveChanges();

                int itemCount = (from kom in db.Komentari where kom.Obrisan == false select kom).Count();

                if (itemCount == 0)
                {
                    btnPrev.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    KomGrid.ItemsSource = null;
                    return;
                }

                if (itemCount % PageSize == 0)
                {
                    MaxPageSize = itemCount / PageSize;
                }
                else
                {
                    MaxPageSize = itemCount / PageSize + 1;
                }

                if (CurrentPageNum > MaxPageSize)
                {
                    OnPreviousClicked(sender, e);
                }
                else
                {
                    CurrentPageNum--;
                    OnNextClicked(sender, e);
                }
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

            if (KorGrid.SelectedItem is Organizator)
            {
                Console.WriteLine((Korisnik)KorGrid.SelectedItem);
                var w = new DodavanjeOrganizatora((Organizator)KorGrid.SelectedItem, KorGrid);
                w.ShowDialog();
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DependencyObject)
            {
                string str = HelpProvider.GetHelpKey((DependencyObject)sender);
                HelpProvider.ShowHelp(str, this, true);
            }
        }

        private void RowSaradnik_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure row was clicked and not empty space
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            if (SarGrid.SelectedItem is Saradnik)
            {
                
                var w = new AzurirajSaradnika((Saradnik)SarGrid.SelectedItem);
                w.ShowDialog();
            }
            using (var db = new DatabaseContext())
            {
                Saradnici = new ObservableCollection<Saradnik>((from sar in db.Saradnici where sar.Obrisan == false select sar));
                SarGrid.ItemsSource = Saradnici;
            }
        }

    }
}
