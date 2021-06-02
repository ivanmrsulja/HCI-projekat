using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
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
    /// Interaction logic for PregledAktuelneManifestacije.xaml
    /// </summary>
    public partial class PregledAktuelneManifestacije : Window, INotifyPropertyChanged
    {
        private Manifestacija _manifestacija;
        public ObservableCollection<Komentar> Komentari { get; set; }

        private ObservableCollection<Ponuda> _ponude;

        public event PropertyChangedEventHandler PropertyChanged;

        public PregledAktuelneManifestacije(Manifestacija current)
        {
            InitializeComponent();
            DataContext = this;
            Manifestacija = current;
            using (var db = new DatabaseContext())
            {
                Manifestacija = (from man in db.Manifestacije.Include("Organizator") where man.Id == Manifestacija.Id select man).FirstOrDefault();
                Ponude = new ObservableCollection<Ponuda>((from man in db.Manifestacije where man.Id == Manifestacija.Id select man.Ponude).FirstOrDefault().ToList());
                Komentari = new ObservableCollection<Komentar>((from kom in db.Komentari where kom.Manifestacija.Id == current.Id && kom.Obrisan != true select kom).ToList());
            }
            if (Manifestacija.MestoOdrzavanjaDone && Manifestacija.BudzetDone && Manifestacija.TemaDone && Manifestacija.GostiDone && Manifestacija.RasporedDone && Manifestacija.DekoracijaDone && Manifestacija.MuzikaDone && Manifestacija.DodatnoDone && Manifestacija.DatumDone && Manifestacija.Status == StatusManifestacije.U_IZRADI && Manifestacija.PredlozenoZaZavrsavanje == false)
            {
                predlozi.IsEnabled = true;
            }
            ukupnaCena.Content = (from p in Ponude select p.Cena).Sum();
            if (Manifestacija.FiksanBudzet)
            {
                tipBudzeta.Content = "(fiksno)";
            }

            

        }

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

        public ObservableCollection<Ponuda> Ponude 
        {
            get
            {
                return _ponude;
            }
            set
            {
                if (value != _ponude)
                {
                    _ponude = value;
                    OnPropertyChanged("Ponude");
                }
            }
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void Nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Predlozi_Click(object sender, RoutedEventArgs e)
        {
            using(var db = new DatabaseContext())
            {
                Manifestacija toUpdate = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                toUpdate.PredlozenoZaZavrsavanje = true;
                Notifikacija nova = new Notifikacija(toUpdate.Organizator.Ime + " " + toUpdate.Organizator.Prezime, "Manifestacija spremna za uvid.(" + toUpdate.Tema + ")", toUpdate.Klijent, toUpdate.Id);
                db.Notifikacije.Add(nova);
                db.SaveChanges();
                Manifestacija = toUpdate;
                predlozi.IsEnabled = false;
                new Thread(() => {
                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("isamrstim06@gmail.com");
                        mail.To.Add(toUpdate.Klijent.Email);
                        mail.Subject = "Manifestacija spremna za uvid";
                        mail.Body = "Postovani " + toUpdate.Klijent.Ime + " " + toUpdate.Klijent.Prezime
                            + ",\nObavestavamo Vas da je ponuda za manifestaciju [" + toUpdate.Tema + "] zakazana za "
                            + toUpdate.DatumOdrzavanja.Date.ToString().Split(' ')[0] + " kod organizatora " + toUpdate.Organizator.Ime + " "
                            + toUpdate.Organizator.Prezime + " spremna za uvid.\n\nSrdacan pozdrav,\nTim 5.1";

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("isamrstim06@gmail.com", "isamrs123");
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);
                    }
                    catch (Exception)
                    {
                        var we = new OkForm("Mail nije bilo moguće poslati\nklijentu(greška u adresi).", "Neuspelo slanje mail-a");
                        we.ShowDialog();
                    }
                }).Start();
            }
            var wk = new OkForm("Manifestacija je poslata\nna uvid klijentu.", "Predlog poslat");
            wk.ShowDialog();
        }

        public void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            var w = new PregledPonuda(Manifestacija, ponude);
            w.ShowDialog();
            if (Manifestacija.MestoOdrzavanjaDone && Manifestacija.BudzetDone && Manifestacija.TemaDone && Manifestacija.GostiDone && Manifestacija.RasporedDone && Manifestacija.DekoracijaDone && Manifestacija.MuzikaDone && Manifestacija.DodatnoDone && Manifestacija.DatumDone && Manifestacija.Status == StatusManifestacije.U_IZRADI && Manifestacija.PredlozenoZaZavrsavanje == false)
            {
                predlozi.IsEnabled = true;
            }
            ukupnaCena.Content = (from p in (ObservableCollection<Ponuda>)ponude.ItemsSource select p.Cena).Sum();
        }

        public void Rasporedi_Click(object sender, RoutedEventArgs e)
        {
            var w = new RasporedjivanjeGostiju(Manifestacija);
            w.ShowDialog();
            if (Manifestacija.MestoOdrzavanjaDone && Manifestacija.BudzetDone && Manifestacija.TemaDone && Manifestacija.GostiDone && Manifestacija.RasporedDone && Manifestacija.DekoracijaDone && Manifestacija.MuzikaDone && Manifestacija.DodatnoDone && Manifestacija.DatumDone && Manifestacija.Status == StatusManifestacije.U_IZRADI && Manifestacija.PredlozenoZaZavrsavanje == false)
            {
                predlozi.IsEnabled = true;
            }
        }

        public void UkloniPonudu_Click(object sender, RoutedEventArgs e)
        {
            Ponuda selected = (Ponuda)ponude.SelectedItem;
            var wq = new YesNo("Da li ste sigurni da\nželite da uklonite ponudu\n" + selected.Opis + "\niz manifestacije?", 0, "Potvrda uklanjanja");
            wq.ShowDialog();

            if (wq.Result != MessageBoxResult.Yes)
            {
                return;
            }
            
            using (var db = new DatabaseContext())
            {
                Ponuda toRemove = (from pon in db.Ponude where pon.Id == selected.Id select pon).FirstOrDefault();
                Manifestacija toUpdate = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                toUpdate.RemovePonuda(toRemove);
                if(toRemove.Stolovi.Count > 0)
                {
                    foreach (Gost gost in toUpdate.Gosti)
                    {
                        foreach (Sto sto in toRemove.Stolovi)
                        {
                            if (sto.BrojStola == gost.BrojStola)
                            {
                                gost.BrojStola = 0;
                            }
                        }
                    }
                }
                toUpdate.PredlozenoZaZavrsavanje = false;
                Manifestacija.PredlozenoZaZavrsavanje = false;
                db.SaveChanges();
                Ponude = new ObservableCollection<Ponuda>((from man in db.Manifestacije where man.Id == Manifestacija.Id select man.Ponude).FirstOrDefault().ToList());
                ponude.ItemsSource = Ponude;
                if (Manifestacija.MestoOdrzavanjaDone && Manifestacija.BudzetDone && Manifestacija.TemaDone && Manifestacija.GostiDone && Manifestacija.RasporedDone && Manifestacija.DekoracijaDone && Manifestacija.MuzikaDone && Manifestacija.DodatnoDone && Manifestacija.DatumDone && Manifestacija.Status == StatusManifestacije.U_IZRADI && Manifestacija.PredlozenoZaZavrsavanje == false)
                {
                    predlozi.IsEnabled = true;
                }
            }
            ukupnaCena.Content = (from p in Ponude select p.Cena).Sum();
            var wk = new OkForm("Ponuda je uklonjena\niz manifestacije.", "Ponuda uklonjena");
            wk.ShowDialog();
        }

        public void Komentarisi_Click(object sender, RoutedEventArgs e)
        {
            int idm = Manifestacija.Id;
            if (noviKomentar.Text.Trim() == "")
            {
                return;
            }
            using (var db = new DatabaseContext())
            {
                Komentar novi = new Komentar(noviKomentar.Text, null, null, DateTime.Now);
                db.Komentari.Add(novi);
                Manifestacija current = (from man in db.Manifestacije.Include("Organizator") where man.Id == Manifestacija.Id select man).FirstOrDefault();
                Manifestacija manif = db.Manifestacije.SingleOrDefault(b => b.Id == idm);
                manif.AddKomentar(novi);
                current.Organizator.AddKomentar(novi);
                db.SaveChanges();
                List<Komentar> komentari = (from kom in db.Komentari where kom.Manifestacija.Id == Manifestacija.Id && kom.Obrisan == false select kom).ToList();
                Komentari = new ObservableCollection<Komentar>(komentari);
            }
            komentariList.ItemsSource = Komentari;
            noviKomentar.Text = "";
        }

        public void Check_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject dpobj = sender as DependencyObject;
            CheckBox cb = sender as CheckBox;
            string name = dpobj.GetValue(FrameworkElement.NameProperty) as string;
            bool? check = cb.IsChecked;
            UpdateCompletion(name, (bool)check);
        }

        private void UpdateCompletion(string name, bool check)
        {
            using (var db = new DatabaseContext())
            {
                Manifestacija toUpdate = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                switch (name)
                {
                    case "mestoCheck":
                        toUpdate.MestoOdrzavanjaDone = !toUpdate.MestoOdrzavanjaDone;
                        break;
                    case "budzetCheck":
                        toUpdate.BudzetDone = !toUpdate.BudzetDone;
                        break;
                    case "gostiCheck":
                        toUpdate.GostiDone = !toUpdate.GostiDone;
                        break;
                    case "temaCheck":
                        toUpdate.TemaDone = !toUpdate.TemaDone;
                        break;
                    case "dekoracijaCheck":
                        toUpdate.DekoracijaDone = !toUpdate.DekoracijaDone;
                        break;
                    case "muzikaCheck":
                        toUpdate.MuzikaDone = !toUpdate.MuzikaDone;
                        break;
                    case "dodatnoCheck":
                        toUpdate.DodatnoDone = !toUpdate.DodatnoDone;
                        break;
                    case "datumCheck":
                        toUpdate.DatumDone = !toUpdate.DatumDone;
                        break;
                    case "rasporedCheck":
                        toUpdate.RasporedDone = !toUpdate.RasporedDone;
                        break;
                }
                if (check == false)
                {
                    toUpdate.PredlozenoZaZavrsavanje = false;
                }
                db.SaveChanges();
                Manifestacija = toUpdate;
                if (Manifestacija.MestoOdrzavanjaDone && Manifestacija.BudzetDone && Manifestacija.TemaDone && Manifestacija.GostiDone && Manifestacija.RasporedDone && Manifestacija.DekoracijaDone && Manifestacija.MuzikaDone && Manifestacija.DodatnoDone && Manifestacija.DatumDone && Manifestacija.Status == StatusManifestacije.U_IZRADI)
                {
                    predlozi.IsEnabled = true;
                }
                else
                {
                    predlozi.IsEnabled = false;
                }
            }
            if (check == true)
            {
                var wk = new OkForm("Obeležili ste stavku\nkao završeno.", "Obeležena stavka");
                wk.ShowDialog();
            }
            else
            {
                var wk = new OkForm("Obeležili ste stavku\nkao nezavršeno.", "Obeležena stavka");
                wk.ShowDialog();
            }
        }

        private void Grid_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
