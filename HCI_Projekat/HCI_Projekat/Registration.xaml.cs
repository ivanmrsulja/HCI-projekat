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

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window, INotifyPropertyChanged
    {
        //private MainWindow oldForm=null;
        private string _email;
        private string _telefon;
        private string _username;
        private string _ime, _prezime, _adresa, _pass, _passConf;

        public event PropertyChangedEventHandler PropertyChanged;

        //public Registration(MainWindow form)
        //{
        //    oldForm = form;
        //}

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

        public string Ime
        {
            get
            {
                return _ime;
            }
            set
            {
                if (value != _ime)
                {
                    _ime = value;
                    OnPropertyChanged("Ime");
                }
            }
        }

        public string Prezime
        {
            get
            {
                return _prezime;
            }
            set
            {
                if (value != _prezime)
                {
                    _prezime = value;
                    OnPropertyChanged("Prezime");
                }
            }
        }

        public string Adresa
        {
            get
            {
                return _adresa;
            }
            set
            {
                if (value != _adresa)
                {
                    _adresa = value;
                    OnPropertyChanged("Adresa");
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

        public string PassConf
        {
            get
            {
                return _passConf;
            }
            set
            {
                if (value != _passConf)
                {
                    _passConf = value;
                    OnPropertyChanged("PassConf");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wk = new YesNo("da li ste sigurni \nda zelite pomoc");
            wk.ShowDialog();

            if (wk.Result == MessageBoxResult.Yes)
            {
                //sta se radi na klik da                
            }
            else
            {
                //sta se radi na klik ne              
            }

            var wka = new OkForm("kliknuli ste na \npomoc");
            wka.ShowDialog();
        }

        public Registration()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Registracija(object sender, RoutedEventArgs e)
        {
            if (user.Text.Trim() == "" || pass.Password.Trim() == "" || passConf.Password.Trim() == "" || ime.Text.Trim() == "" || prezime.Text.Trim() == "" || email.Text.Trim() == "" || telefon.Text.Trim() == "" || adresa.Text.Trim() == "")
            {
                MessageBox.Show("Molimo popunite sva polja i pokusajte ponovo.", "Nisu popunjena sva polja");
                return;
            }
            if(pass.Password != passConf.Password)
            {
                MessageBox.Show("Lozinka u polju 'Potvrdi lozinku' mora biti ista kao ona u polju 'Lozinka'.", "Lozinke se ne podudaraju");
                return;
            }
            using(var db = new DatabaseContext())
            {
                Klijent novi = new Klijent(user.Text, pass.Password, ime.Text, prezime.Text, email.Text, telefon.Text, adresa.Text);
                db.Korisnici.Add(novi);
                db.SaveChanges();
            }
            MessageBox.Show("Uspesno ste kreirali nalog, mozete se ulogovati.", "Uspesno kreiran nalog");
            this.Hide();
        }
        public void Odustani(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void FormStateChanged(object sender, RoutedEventArgs e)
        {
            Regex regexTel = new Regex(@"[0][0-9]{9}");
            Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match matchTel = regexTel.Match(telefon.Text);
            Match matchEmail = regexEmail.Match(email.Text);
            if (String.IsNullOrWhiteSpace(pass.Password) || String.IsNullOrWhiteSpace(passConf.Password) || user.Text.Trim() == "" || ime.Text.Trim() == "" || prezime.Text.Trim() == "" || email.Text.Trim() == "" || telefon.Text.Trim() == "" || adresa.Text.Trim() == "" || !matchTel.Success || !matchEmail.Success || pass.Password != passConf.Password)
                registrujSe.IsEnabled = false;
            else
            {
                using (var db = new DatabaseContext())
                {
                    string[] usernames = (from users in db.Korisnici where users.Username == user.Text select users.Username).ToArray();
                    if (usernames.Length != 0)
                    {
                        registrujSe.IsEnabled = false;
                        return;
                    }
                }
                registrujSe.IsEnabled = true;
            }
                
        }
    }
}
