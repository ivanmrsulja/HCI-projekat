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
                foreach (var entity in db.Stolovi)
                    db.Stolovi.Remove(entity);

                Komentar ko1 = new Komentar("Sve je super, svaka cast!!! :)", null, null, DateTime.ParseExact("20-04-2021", "dd-MM-yyyy", null));
                Komentar ko2 = new Komentar("Htio sam Gocija a dobio sam Lapsus bend! Hocu pare nazad!", null, null, DateTime.Now);
                db.Komentari.Add(ko1);
                db.Komentari.Add(ko2);

                Manifestacija man1 = new Manifestacija(TemaManifestacije.VENČANJE, 3000, true, 600, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact("20/06/2021", "dd/MM/yyyy", null), null, null);
                man1.Status = StatusManifestacije.IZRADA;
                Manifestacija man2 = new Manifestacija(TemaManifestacije.ROĐENDAN, 10000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact("25/06/2021", "dd/MM/yyyy", null), null, null);
                man2.Status = StatusManifestacije.ZAVRŠENA;
                Manifestacija man3 = new Manifestacija(TemaManifestacije.VENČANJE, 3000, true, 600, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact("14/06/2021", "dd/MM/yyyy", null), null, null);
                Manifestacija man4 = new Manifestacija(TemaManifestacije.ROĐENDAN, 10000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact("05/06/2021", "dd/MM/yyyy", null), null, null);
                Manifestacija man5 = new Manifestacija(TemaManifestacije.VENČANJE, 3000, true, 600, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact("13/09/2021", "dd/MM/yyyy", null), null, null);
                Manifestacija man6 = new Manifestacija(TemaManifestacije.ROĐENDAN, 10000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.ParseExact("31/07/2021", "dd/MM/yyyy", null), null, null);
                man1.AddKomentar(ko1);
                man1.AddKomentar(ko2);
                db.Manifestacije.Add(man1);
                db.Manifestacije.Add(man2);
                db.Manifestacije.Add(man3);
                db.Manifestacije.Add(man4);
                db.Manifestacije.Add(man5);
                db.Manifestacije.Add(man6);

                Klijent k1 = new Klijent("k", "k", "Nikola", "Petrovic", "isamrstim06@gmail.com", "0000000000000", "adresa");
                k1.AddManifestacija(man1);
                k1.AddManifestacija(man2);
                k1.AddManifestacija(man3);
                k1.AddManifestacija(man4);
                k1.AddKomentar(ko1);
                k1.AddKomentar(ko2);
                db.Korisnici.Add(k1);

                Klijent k2 = new Klijent("petar", "petar", "Petar", "Markovic", "isamrstim06@gmail.com", "0000000000000", "adresa");
                k2.AddManifestacija(man5);
                k2.AddManifestacija(man6);
                db.Korisnici.Add(k2);

                Organizator o1 = new Organizator("o", "o", "Ana", "Jovovic", "email@email.com", "0685478521", "adresa");
                o1.AddManifestacija(man1);
                o1.AddManifestacija(man2);
                db.Korisnici.Add(o1);

                Organizator o2 = new Organizator("dusan", "dusan", "Dusan", "Antic", "email@email.com", "0685478521", "adresa");
                o2.AddManifestacija(man3);
                o2.AddManifestacija(man4);
                db.Korisnici.Add(o2);

                Admin a1 = new Admin("admin", "admin", "Joko", "Joksimovic", "email@email.com", "0685478521", "adresa");
                db.Korisnici.Add(a1);

                Admin a2 = new Admin("ivan", "ivan", "Ivan", "Mrsulja", "email@email.com", "0685478521", "adresa");
                db.Korisnici.Add(a2);

                Saradnik s1 = new Saradnik("Restoran1", "Jevrejska 12", TipSaradnika.RESTORAN, "Dobra hrana", "../../Images/map_1.png");
                Saradnik s3 = new Saradnik("Restoran2", "Lasla Gala 12", TipSaradnika.RESTORAN, "Bolja hrana", "../../Images/map_2.png");
                Saradnik s2 = new Saradnik("Fotograf Jovo", "Ruzin Gaj 12", TipSaradnika.FOTOGRAF, "Slikam za instagram", "ne treba link do mape");
                db.Saradnici.Add(s1);
                db.Saradnici.Add(s2);
                db.Saradnici.Add(s3);

                Sto sto1 = new Sto(4, 1);
                Sto sto2 = new Sto(4, 2);
                Sto sto3 = new Sto(4, 3);
                Sto sto4 = new Sto(7, 4);
                Sto sto5 = new Sto(4, 4);
                Sto sto6 = new Sto(4, 5);
                Sto sto7 = new Sto(4, 6);
                Sto sto8 = new Sto(4, 7);
                Sto sto9 = new Sto(4, 8);
                Sto sto10 = new Sto(4, 9);
                Sto sto11 = new Sto(4, 10);
                Sto sto12 = new Sto(4, 11);
                Sto sto13 = new Sto(4, 12);
                Sto sto14 = new Sto(4, 13);
                Sto sto15 = new Sto(4, 14);
                Sto sto16 = new Sto(4, 15);

                db.Stolovi.Add(sto1);
                db.Stolovi.Add(sto2);
                db.Stolovi.Add(sto3);
                db.Stolovi.Add(sto4);
                db.Stolovi.Add(sto5);
                db.Stolovi.Add(sto6);
                db.Stolovi.Add(sto7);

                Ponuda pon1 = new Ponuda("Gornji dio", 500, s1);
                Ponuda pon2 = new Ponuda("Sredina restorana", 1200, s1);
                Ponuda pon4 = new Ponuda("Separe", 300, s3);
                Ponuda pon3 = new Ponuda("Fotosuting", 100, s2);

                Ponuda pon5 = new Ponuda("Donji dio", 600, s1);

                db.Ponude.Add(pon1);
                db.Ponude.Add(pon2);
                db.Ponude.Add(pon3);
                db.Ponude.Add(pon4);
                db.Ponude.Add(pon5);
                pon1.Stolovi.Add(sto1);
                pon1.Stolovi.Add(sto2);
                pon1.Stolovi.Add(sto3);
                pon2.Stolovi.Add(sto5);
                pon2.Stolovi.Add(sto6);
                pon2.Stolovi.Add(sto7);
                pon2.Stolovi.Add(sto8);
                pon2.Stolovi.Add(sto9);
                pon2.Stolovi.Add(sto10);
                pon2.Stolovi.Add(sto11);
                pon2.Stolovi.Add(sto12);
                pon4.Stolovi.Add(sto4);

                pon5.Stolovi.Add(sto13);
                pon5.Stolovi.Add(sto14);
                pon5.Stolovi.Add(sto15);
                pon5.Stolovi.Add(sto16);

                s1.AddPonuda(pon1);
                s1.AddPonuda(pon2);
                s1.AddPonuda(pon5);
                s2.AddPonuda(pon3);
                s3.AddPonuda(pon4);

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

                Gost gost1 = new Gost("Ivan Ivanovic", 0, null);
                Gost gost2 = new Gost("Marko Markovic", 0, null);
                Gost gost3 = new Gost("Zivko Zivkovic", 0, null);
                Gost gost4 = new Gost("Petar Petrovic", 1, null);
                Gost gost5 = new Gost("Nikola Nikolic", 0, null);
                Gost gost6 = new Gost("Marija Ivanovic", 1, null);
                Gost gost7 = new Gost("Nina Djukanovic", 3, null);
                Gost gost8 = new Gost("Nadja Nedovic", 0, null);
                Gost gost9 = new Gost("Jelena Jevtovic", 0, null);
                Gost gost10 = new Gost("Jelena Jevtovic", 4, null);
                db.Gosti.Add(gost1);
                db.Gosti.Add(gost2);
                db.Gosti.Add(gost3);
                db.Gosti.Add(gost4);
                db.Gosti.Add(gost5);
                db.Gosti.Add(gost6);
                db.Gosti.Add(gost7);
                db.Gosti.Add(gost8);
                db.Gosti.Add(gost9);
                db.Gosti.Add(gost10);

                man1.AddGost(gost1);
                man1.AddGost(gost2);
                man1.AddGost(gost3);
                man1.AddGost(gost4);
                man1.AddGost(gost5);
                man1.AddGost(gost6);
                man1.AddGost(gost7);
                man1.AddGost(gost8);
                man1.AddGost(gost9);

                man2.AddGost(gost10);

                db.SaveChanges();

                Notifikacija not1 = new Notifikacija("Ana Jovovic", "Manifestacija spremna za uvid. (VENČANJE)", k1, (from m in db.Manifestacije select m).ToArray()[0].Id);
                Notifikacija not2 = new Notifikacija("Ana Jovovic", "Manifestacija spremna za uvid. (ROĐENDAN)", k1, (from m in db.Manifestacije select m).ToArray()[1].Id);
                Notifikacija not3 = new Notifikacija("Ana Jovovic", "Manifestacija spremna za uvid. (ROĐENDAN)", k1, (from m in db.Manifestacije select m).ToArray()[1].Id);
                db.Notifikacije.Add(not1);
                db.Notifikacije.Add(not2);
                db.Notifikacije.Add(not3);
                k1.AddNotifikacija(not1);
                k1.AddNotifikacija(not2);
                k1.AddNotifikacija(not3);

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
                    var wk = new OkForm("Neispravno korisničko\nime ili lozinka.", "Neuspela prijava");
                    wk.ShowDialog();
                    return;
                }
                else
                {
                    if(currentUser[0].Password != pass.Password || currentUser[0].Username != user.Text)
                    {
                        var wk = new OkForm("Neispravno korisničko\nime ili lozinka.", "Neuspela prijava");
                        wk.ShowDialog();
                        return;
                    }
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
            user.Clear();
            pass.Clear();
        }

        public void RegistrujSe(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var w = new Registration();
            w.ShowDialog();
            this.Show();
        }

        public void Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UlogujSe(sender, e);
            }
        }
    }

}
