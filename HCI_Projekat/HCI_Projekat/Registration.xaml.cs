using HCI_Projekat.Model;
using System;
using System.Collections.Generic;
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

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window, INotifyPropertyChanged
    {
        private string _email;
        private string _telefon;
        private string _username;

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

        public Registration()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Registracija(object sender, RoutedEventArgs e)
        {
            using(var db = new Context())
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
    }
}
