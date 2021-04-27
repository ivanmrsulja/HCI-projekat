using HCI_Projekat.Administrator;
using HCI_Projekat.KlijentView;
using HCI_Projekat.Model;
using HCI_Projekat.OrganizatorView;
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
            using (var db = new Context())
            {
                foreach (var entity in db.Korisnici)
                    db.Korisnici.Remove(entity);
                foreach (var entity in db.Manifestacije)
                    db.Manifestacije.Remove(entity);
                foreach (var entity in db.Komentari)
                    db.Komentari.Remove(entity);

                Komentar ko1 = new Komentar("aaaaa", null, null);
                db.Komentari.Add(ko1);

                Manifestacija man1 = new Manifestacija("venacanje", 3000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.Now, null, null);
                Manifestacija man2 = new Manifestacija("venacanje", 3000, true, 500, "restoran", "cvece i baloni", "goci", "nema", DateTime.Now, null, null);
                man1.AddKomentar(ko1);
                db.Manifestacije.Add(man1);
                db.Manifestacije.Add(man2);

                Klijent k1 = new Klijent("klijent", "klijent", "Nikola", "Petrovic", "email", "telefon", "adresa");
                k1.AddManifestacija(man1);
                k1.AddManifestacija(man2);
                k1.AddKomentar(ko1);
                db.Korisnici.Add(k1);

                Organizator o1 = new Organizator("organizator", "organizator", "Jovan", "Jovovic", "email", "telefon", "adresa");
                o1.AddManifestacija(man1);
                db.Korisnici.Add(o1);

                Admin a1 = new Admin("admin", "admin", "Joko", "Sompompjerovicjerosolomitipitikovski", "email", "telefon", "adresa");
                db.Korisnici.Add(a1);


                db.SaveChanges();
            }
        }

        public void UlogujSe(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Logovanje");
            string passw = pass.Password;
            string username = user.Text;
            using (var db = new Context())
            {
                Korisnik[] currentUser = (from k in db.Korisnici where k.Username == username && k.Password == passw select k).ToArray();
                if (currentUser.Length == 0)
                {
                    MessageBox.Show("Neispravno korisnicko ime ili lozinka.", "Obavestenje");
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
                            var wk = new KlijentHOME(this);
                            wk.ShowDialog();
                            break;
                        case UlogaKorisnika.ORGANIZATOR:
                            var wo = new OrganizatorHOME(this);
                            wo.ShowDialog();
                            break;
                    }
                }
            }
        }

        public void RegistrujSe(object sender, RoutedEventArgs e)
        {
            
            var w = new Registration();
            w.ShowDialog();
            
        }
    }

}
