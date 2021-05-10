using HCI_Projekat.Administrator;
using HCI_Projekat.KlijentView;
using HCI_Projekat.Model;
using HCI_Projekat.OrganizatorView;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new DatabaseContext())
            {
                foreach (var entity in db.Korisnici)
                    db.Korisnici.Remove(entity);
                foreach (var entity in db.Manifestacije)
                    db.Manifestacije.Remove(entity);
                foreach (var entity in db.Komentari)
                    db.Komentari.Remove(entity);
                foreach (var entity in db.Gosti)
                    db.Gosti.Remove(entity);
                foreach (var entity in db.Ponude)
                    db.Ponude.Remove(entity);
                foreach (var entity in db.Saradnici)
                    db.Saradnici.Remove(entity);
                foreach (var entity in db.Notifikacije)
                    db.Notifikacije.Remove(entity);

                Komentar ko1 = new Komentar("Sve je super, svaka cast!!! :)", null, null, DateTime.ParseExact("20-04-2021", "dd-MM-yyyy", null));
                Komentar ko2 = new Komentar("Htio sam Gocija a dobio sam [DATA EXPUNGED]! Ocu pare nazad!", null, null, DateTime.Now);
                db.Komentari.Add(ko1);
                db.Komentari.Add(ko2);
                string text = "20/04/2021";
                Manifestacija man1 = new Manifestacija(TemaManifestacije.VENCANJE, 3000, true, 600, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact(text, "dd/MM/yyyy", null), null, null);
                man1.Status = StatusManifestacije.U_IZRADI;
                Manifestacija man2 = new Manifestacija(TemaManifestacije.RODJENDAN, 10000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact(text, "dd/MM/yyyy", null), null, null);
                man2.Status = StatusManifestacije.ZAVRSENA;
                Manifestacija man3 = new Manifestacija(TemaManifestacije.VENCANJE, 3000, true, 600, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact(text, "dd/MM/yyyy", null), null, null);
                Manifestacija man4 = new Manifestacija(TemaManifestacije.RODJENDAN, 10000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact(text, "dd/MM/yyyy", null), null, null);
                Manifestacija man5 = new Manifestacija(TemaManifestacije.VENCANJE, 3000, true, 600, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact(text, "dd/MM/yyyy", null), null, null);
                Manifestacija man6 = new Manifestacija(TemaManifestacije.RODJENDAN, 10000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact(text, "dd/MM/yyyy", null), null, null);
                man1.AddKomentar(ko1);
                man1.AddKomentar(ko2);
                db.Manifestacije.Add(man1);
                db.Manifestacije.Add(man2);
                db.Manifestacije.Add(man3);
                db.Manifestacije.Add(man4);
                db.Manifestacije.Add(man5);
                db.Manifestacije.Add(man6);

                Klijent k1 = new Klijent("k", "k", "Nikola", "Petrovic", "email@gmail.com", "0000000000000", "adresa");
                k1.AddManifestacija(man1);
                k1.AddManifestacija(man2);
                k1.AddManifestacija(man3);
                k1.AddManifestacija(man4);
                k1.AddManifestacija(man5);
                k1.AddManifestacija(man6);
                k1.AddKomentar(ko1);
                k1.AddKomentar(ko2);
                db.Korisnici.Add(k1);

                Organizator o1 = new Organizator("o", "o", "Ana", "Jovovic", "email@email.com", "0685478521", "adresa");
                o1.AddManifestacija(man1);
                o1.AddManifestacija(man2);
                db.Korisnici.Add(o1);

                Admin a1 = new Admin("admin", "admin", "Joko", "Sompompjerovicjerosolomitipitikovski", "email@email.com", "0685478521", "adresa");
                db.Korisnici.Add(a1);

                Saradnik s1 = new Saradnik("Restoran1", "Jevrejska 12", TipSaradnika.RESTORAN, "Dobra hrana", "link do mape");
                Saradnik s2 = new Saradnik("Fotograf Jovo", "Ruzin Gaj 12", TipSaradnika.FOTOGRAF, "Slikam za instagram", "ne treba link do mape");
                db.Saradnici.Add(s1);
                db.Saradnici.Add(s2);

                Ponuda pon1 = new Ponuda("Velika sala", 500, s1);
                Ponuda pon2 = new Ponuda("Mala sala", 200, s1);
                Ponuda pon3 = new Ponuda("Fotosuting", 100, s2);
                db.Ponude.Add(pon1);
                db.Ponude.Add(pon2);
                db.Ponude.Add(pon3);

                s1.AddPonuda(pon1);
                s1.AddPonuda(pon2);
                s2.AddPonuda(pon3);

                man1.AddPonuda(pon1);
                man1.AddPonuda(pon3);
                man1.MestoOdrzavanjaDone = true;
                man1.DodatnoDone = true;
                man1.BudzetDone = true;
                man1.DatumDone = true;
                man1.DekoracijaDone = true;
                man1.MuzikaDone = true;
                man1.RasporedDone = true;
                man1.TemaDone = true;
                man1.GostiDone = true;

                man2.AddPonuda(pon2);
                man2.MestoOdrzavanjaDone = true;
                man2.DodatnoDone = true;
                man2.BudzetDone = true;
                man2.DatumDone = true;
                man2.DekoracijaDone = true;
                man2.MuzikaDone = true;
                man2.RasporedDone = true;
                man2.TemaDone = true;
                man2.GostiDone = true;

                db.SaveChanges();
            }
        }

        public void UlogujSe(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Logovanje");
            string passw = pass.Password;
            string username = user.Text;
            using (var db = new DatabaseContext())
            {
                Korisnik[] currentUser = (from k in db.Korisnici where k.Username == username && k.Password == passw select k).ToArray();
                if (currentUser.Length == 0)
                {
                    var wk = new OkForm("Neispravno korisnicko\nime ili lozinka.", "Neuspela prijava");
                    wk.ShowDialog();
                }
                else
                {
                    Console.WriteLine(currentUser[0].Uloga);
                    switch (currentUser[0].Uloga)
                    {
                        case UlogaKorisnika.ADMIN:
                            var wa = new HomeAdmin(this);
                            wa.ShowDialog();
                            break;
                        case UlogaKorisnika.KLIJENT:
                            var wk = new KlijentHOME(this, currentUser[0]);
                            wk.ShowDialog();
                            break;
                        case UlogaKorisnika.ORGANIZATOR:
                            var wo = new OrganizatorHOME(this, currentUser[0]);
                            wo.ShowDialog();
                            break;
                    }
                }
            }
        }

        public void RegistrujSe(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var w = new Registration();
            w.ShowDialog();
            this.Show();
        }
    }

}
