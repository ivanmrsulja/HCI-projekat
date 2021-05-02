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
    /// Interaction logic for DodajManifestaciju.xaml
    /// </summary>
    public partial class DodajManifestaciju : Window
    {
        public Klijent Klijent { get; set; }

        public DodajManifestaciju(Klijent k)
        {
            InitializeComponent();
            Klijent = k;
        }

        private void FormStateChanged(object sender, RoutedEventArgs e)
        {

            int brGosti = 0;
            double budz = 0;
            if (brojGostiju.Text!="")
            {
                try
                {
                    brGosti = Int32.Parse(brojGostiju.Text);
                }
                catch
                {
                    potvrdi.IsEnabled = false;
                }
            }
            if (budzet.Text != "")
            {
                try
                {
                    budz = Double.Parse(budzet.Text);
                }
                catch
                {
                    potvrdi.IsEnabled = false;
                }
            }
            if (tema.Text.Trim() == "" || mesto.Text.Trim() == "" || muzika.Text.Trim() == "" || dekoracije.Text.Trim() == ""  || budz<1|| brGosti< 1 || datum.Text.Trim() == ""|| dodatniZahtevi.Text.Trim() == "")
                potvrdi.IsEnabled = false;
            else
            {
                //using (var db = new DatabaseContext())
                //{
                //    string[] manifestacije = (from man in db.Manifestacije where man.Username == user.Text select users.Username).ToArray();
                //    if (usernames.Length != 0)
                //    {
                //        registrujSe.IsEnabled = false;
                //        return;
                //    }
                //}
                potvrdi.IsEnabled = true;
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Potvrdi_Click(object sender, RoutedEventArgs e)
        {
            TemaManifestacije t = stringToManifestacija(tema.Text.ToUpper());
            bool c = (fiksan.IsChecked == true) ? true : false;
            var d = Convert.ToDateTime(datum.Text).ToString("MM/dd/yyyy");
            using (var db = new DatabaseContext())
            {               
                Manifestacija novi = new Manifestacija(t,Double.Parse(budzet.Text),c,Int32.Parse(brojGostiju.Text),mesto.Text,dekoracije.Text,muzika.Text,dodatniZahtevi.Text,Convert.ToDateTime(d), null,Klijent);
                db.Manifestacije.Add(novi);
                db.SaveChanges();
            }
            var dijalog5 = new OkForm("Uspesno ste kreirali\nmanifestaciju");
            dijalog5.ShowDialog();
            this.Hide();
        }

        private TemaManifestacije stringToManifestacija(string manUpper)
        {
            if (manUpper == "VENCANJE")
                return TemaManifestacije.VENCANJE;
            else if (manUpper == "RODJENDAN")
                return TemaManifestacije.RODJENDAN;
            else if (manUpper == "KOKTEL PARTI" || manUpper == "KOKTEL PARTY")
                return TemaManifestacije.KOKTEL_PARTY;
            else if (manUpper == "REJV")
                return TemaManifestacije.REJV;
            else if (manUpper == "OTVARANJE")
                return TemaManifestacije.OTVARANJE;
            return TemaManifestacije.SVE;
        }
    }
}
