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
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace HCI_Projekat.Administrator
{
    /// <summary>
    /// Interaction logic for DodavanjeOrganizatora.xaml
    /// </summary>
    public partial class DodavanjeOrganizatora : Window, INotifyPropertyChanged
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

        public Organizator Organizator { get; set; }
        public DataGrid ParentData { get; set; }

        public DodavanjeOrganizatora(Organizator o, DataGrid data)
        {
            InitializeComponent();
            DataContext = this;
            Organizator = o;
            ParentData = data;
            if (Organizator != null)
            {
                ime.Text = Organizator.Ime;
                prezime.Text = Organizator.Prezime;
                Email = Organizator.Email;
                Username = Organizator.Username;
                adresa.Text = Organizator.Adresa;
                Telefon = Organizator.Telefon;
                dodajBtn.Content = "SAČUVAJ";
                HintAssist.SetHint(passConf, "NOVA LOZINKA");
                user.IsReadOnly = true;
                naslov.Content = "AŽURIRANJE ORGANIZATORA";
                Title = "Ažuriranje organizatora";
            }

        }

        public void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (Organizator == null && (user.Text.Trim() == "" || pass.Password.Trim() == "" || passConf.Password.Trim() == "" || ime.Text.Trim() == "" || prezime.Text.Trim() == "" || email.Text.Trim() == "" || telefon.Text.Trim() == "" || adresa.Text.Trim() == ""))
            {
                var dijalog3 = new OkForm("Molimo popunite sva polja\ni pokušajte ponovo", "Došlo je do greške");
                dijalog3.ShowDialog();
                return;
            }
            if (pass.Password != passConf.Password && Organizator == null)
            {
                var dijalog4 = new OkForm("Lozinka u polju 'Potvrdi lozinku'\nmora biti ista kao ona u polju 'Lozinka'", "Došlo je do greške");
                dijalog4.ShowDialog();
                return;
            }

            if(Organizator == null)
            {
                using (var db = new DatabaseContext())
                {
                    Organizator novi = new Organizator(user.Text, pass.Password, ime.Text, prezime.Text, email.Text, telefon.Text, adresa.Text);
                    db.Korisnici.Add(novi);
                    db.SaveChanges();
                }

                var dijalog5 = new OkForm("Uspešno ste kreirali\nnovog organizatora.", "Organizator kreiran");
                dijalog5.ShowDialog();
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    var result = db.Korisnici.SingleOrDefault(b => b.Id == Organizator.Id);
                    if (result != null)
                    {
                        result.Ime = ime.Text;
                        result.Prezime = prezime.Text;
                        result.Email = email.Text;
                        result.Adresa = adresa.Text;
                        result.Telefon = telefon.Text;
                        if (passConf.Password != "" && !passConf.Password.Contains(" "))
                        {
                            result.Password = passConf.Password;
                        }
                        else if (passConf.Password.Contains(" "))
                        {
                            var passDijalog = new OkForm("Nova lozinka sadrži\nnedozvoljene karaktere.", "Loš format lozinke");
                            passDijalog.ShowDialog();
                            return;
                        }
                        db.SaveChanges();
                    }
                }

                var dijalog5 = new OkForm("Uspešno ste ažurirali\norganizatora " + Organizator.Username + ".", "Uspešno sačuvano");
                dijalog5.ShowDialog();
            }
            using(var db = new DatabaseContext())
            {
                ParentData.ItemsSource = new ObservableCollection<Korisnik>((from kor in db.Korisnici select kor).ToList());
            }
            this.Close();
        }

        public void Odustani_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Pomoc_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelp("HelpDodajOrganizatora", this);
        }

        private void FormStateChanged(object sender, EventArgs e)
        {
            Regex regexTel = new Regex(@"[0][0-9]{9}");
            Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match matchTel = regexTel.Match(telefon.Text);
            Match matchEmail = regexEmail.Match(email.Text);
            if(Organizator == null)
            {
                if (String.IsNullOrWhiteSpace(pass.Password) || String.IsNullOrWhiteSpace(passConf.Password) || user.Text.Trim() == "" || ime.Text.Trim() == "" || prezime.Text.Trim() == "" || email.Text.Trim() == "" || telefon.Text.Trim() == "" || adresa.Text.Trim() == "" || !matchTel.Success || !matchEmail.Success || pass.Password != passConf.Password)
                    dodajBtn.IsEnabled = false;
                else
                {
                    using (var db = new DatabaseContext())
                    {
                        string[] usernames = (from users in db.Korisnici where users.Username == user.Text select users.Username).ToArray();
                        if (usernames.Length != 0)
                        {
                            dodajBtn.IsEnabled = false;
                            return;
                        }
                    }
                    dodajBtn.IsEnabled = true;
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(pass.Password) || user.Text.Trim() == "" || ime.Text.Trim() == "" || prezime.Text.Trim() == "" || email.Text.Trim() == "" || telefon.Text.Trim() == "" || adresa.Text.Trim() == "" || !matchTel.Success || !matchEmail.Success || pass.Password != Organizator.Password)
                {
                    dodajBtn.IsEnabled = false;
                }   
                else
                {
                    using (var db = new DatabaseContext())
                    {
                        string[] usernames = (from users in db.Korisnici where users.Username == user.Text select users.Username).ToArray();
                        if (usernames.Length != 1)
                        {
                            dodajBtn.IsEnabled = false;
                            return;
                        }
                    }
                    dodajBtn.IsEnabled = true;
                }
            }

           
        }

    }
}
