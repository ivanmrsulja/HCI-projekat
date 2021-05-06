using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Profil : Window, INotifyPropertyChanged
    {

        public Klijent klijent { get; set; }
        public Klijent korisnik { get; set; }
        public object Sender { get; set; }

        private string _email;
        private string _telefon;
        private string _username;
        private string _pass;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public string Telefon
        {
            get
            {
                return _telefon;
            }
            set
            {
                if (value != _telefon)
                {
                    _telefon = value;
                    OnPropertyChanged("Telefon");
                }
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public string Password
        {
            get
            {
                return _pass;
            }
            set
            {
                if (value != _pass)
                {
                    _pass = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        public Profil(Klijent k)
        {
            InitializeComponent();
            DataContext = this;
            klijent = k;
            korisnik = null;
            ime.Text = k.Ime;
            prezime.Text = k.Prezime;
            Username = k.Username;
            Email = k.Email;
            adresa.Text = k.Adresa;
            Telefon = k.Telefon;

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
                    if(passNew.Password != "" && !passNew.Password.Contains(" "))
                    {
                        result.Password = passNew.Password;
                    }
                    else if(passNew.Password.Contains(" "))
                    {
                        var passDijalog = new OkForm("Nova lozinka sadrzi\nnedozvoljene karaktere.", "Los format lozinke");
                        passDijalog.ShowDialog();
                        return;
                    }
                    db.SaveChanges();
                }
            }
            Klijent k1 = new Klijent(user.Text, pass.Password.ToString(), ime.Text, prezime.Text, email.Text, telefon.Text,adresa.Text);
            korisnik = k1;
            var dijalog5 = new OkForm("Uspesno ste azurirali\nprofil.", "Uspesno sacuvano");
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
