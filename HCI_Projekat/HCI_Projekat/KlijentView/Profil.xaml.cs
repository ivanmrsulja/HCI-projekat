using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Profil.xaml
    /// </summary>
    public partial class Profil : Window
    {

        public Klijent klijent { get; set; }
        public Klijent korisnik { get; set; }
        public object Sender { get; set; }


        public Profil(Klijent k)
        {
            InitializeComponent();

            klijent = k;
            korisnik = null;
            ime.Text = k.Ime;
            prezime.Text = k.Prezime;
            user.Text = k.Username;
            email.Text = k.Email;
            adresa.Text = k.Adresa;
            telefon.Text = k.Telefon;

        }       

        private void FormStateChanged(object sender, RoutedEventArgs e)
        {
            Regex regexTel = new Regex(@"[0][0-9]{9}");
            Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match matchTel = regexTel.Match(telefon.Text);
            Match matchEmail = regexEmail.Match(email.Text);
            if (String.IsNullOrWhiteSpace(pass.Password) ||  user.Text.Trim() == "" || ime.Text.Trim() == "" || prezime.Text.Trim() == "" || email.Text.Trim() == "" || telefon.Text.Trim() == "" || adresa.Text.Trim() == "" || !matchTel.Success || !matchEmail.Success || pass.Password != klijent.Password)
                sacuvaj.IsEnabled = false;
            else
            {
                sacuvaj.IsEnabled = true;
            }
        }

        private void Odustani(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DatabaseContext())
            {
                var result = db.Korisnici.SingleOrDefault(b => b.Id == klijent.Id);
                if (result != null)
                {
                    result.Ime = ime.Text;
                    result.Prezime = prezime.Text;
                    result.Username = user.Text;
                    result.Email = email.Text;
                    result.Adresa = adresa.Text;
                    result.Telefon = telefon.Text;
                    db.SaveChanges();
                }
            }
            Klijent k1 = new Klijent(user.Text, pass.Password.ToString(), ime.Text, prezime.Text, email.Text, telefon.Text,adresa.Text);
            korisnik = k1;
            var dijalog5 = new OkForm("Uspesno ste azurirali\nkorisnika");
            dijalog5.ShowDialog();
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        internal object GetValue()
        {
            return korisnik;
        }
    }
}
