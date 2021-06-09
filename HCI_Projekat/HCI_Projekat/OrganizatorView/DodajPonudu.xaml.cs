using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace HCI_Projekat.OrganizatorView
{
    /// <summary>
    /// Interaction logic for DodajPonudu.xaml
    /// </summary>
    public partial class DodajPonudu : Window, INotifyPropertyChanged
    {
        private string FileName;
        string retValue = "";
        private double _cena;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public double Cena
        {
            get
            {
                return _cena;
            }
            set
            {
                if (value != _cena)
                {
                    _cena = value;
                    OnPropertyChanged("Cena");
                }
            }
        }

        public List<Sto> Stolovi { get; set; }
        public Saradnik Saradnik;

        public DodajPonudu(bool res, List<Sto> s, Saradnik sa)
        {
            InitializeComponent();
            DataContext = this;

            Stolovi = s;
            Saradnik = sa;

            if (res == false)
            {
                izaberiFajl.Visibility = Visibility.Hidden;
                imeFajla.Visibility = Visibility.Hidden;
            }
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (opis.Text.Contains("-"))
            {
                var wk = new OkForm("Opis ponude sadrži nedozvoljene karaktere ('-').", "Opis sadrži nedozvoljene karaktere.", true);
                wk.ShowDialog();
                return;
            }
            if (opis.Text.Contains(","))
            {
                var wk = new OkForm("Opis ponude sadrži nedozvoljene karaktere (',').", "Opis sadrži nedozvoljene karaktere.", true);
                wk.ShowDialog();
                return;
            }
            if (Saradnik != null)
            {
                using (var db = new DatabaseContext())
                {
                    Saradnik owner = (from sar in db.Saradnici where sar.Id == Saradnik.Id select sar).FirstOrDefault();
                    foreach (Ponuda p in owner.Ponude)
                    {
                        Console.WriteLine(p.Opis == opis.Text);
                        if (p.Opis.Trim() == opis.Text.Trim())
                        {
                            var wk = new OkForm("Dve ponude kod istog saradnika ne mogu imati isti opis.", "Dupliran opis", true);
                            wk.ShowDialog();
                            return;
                        }
                    }
                }
            }
            retValue = opis.Text + " - " + cena.Text + "," + FileName;
            this.Close();
        }

        public string Ret
        {
            get { return retValue; }
        }

        private void Odustani_Click(object sender, RoutedEventArgs e)
        {
            retValue = "";
            this.Close();
        }

        private void IzaberiFajl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    FileName = openFileDialog.FileName;


                imeFajla.Content = System.IO.Path.GetFileName(FileName);
                if (System.IO.Path.GetFileName(FileName) == null)
                {
                    FileName = "";
                    var wk = new OkForm("Niste izabrali dobar fajl.", "Pogrešan format fajla");
                    wk.ShowDialog();
                    imeFajla.Content = "(AKO JE SALA)";
                    return;
                }
                else if (System.IO.Path.GetFileName(FileName).Split('.')[1] != "txt" && System.IO.Path.GetFileName(FileName).Split('.')[1] != "csv")
                {
                    Console.WriteLine(System.IO.Path.GetFileName(FileName).Split('.')[1]);
                    var wk = new OkForm("Niste izabrali .txt ili .csv fajl.", "Pogrešan format fajla");
                    wk.ShowDialog();
                    FileName = "";
                    imeFajla.Content = "(AKO JE SALA)";
                    return;
                }

                string[] lines = System.IO.File.ReadAllLines(FileName);

                foreach(string line in lines)
                {
                    string[] tokens = line.Split(',');
                    if(tokens.Count() != 2)
                    {
                        var wk = new OkForm("Pogrešna struktura fajla.", "Greška u učitavanju");
                        wk.ShowDialog();
                        FileName = "";
                        imeFajla.Content = "(AKO JE SALA)";
                        return;
                    }
                    try
                    {
                        Sto novi = new Sto(Int32.Parse(tokens[0]), Int32.Parse(tokens[1]));
                        Stolovi.Add(novi);
                    }
                    catch
                    {
                        var wk = new OkForm("Vrednosti u fajlu su pogrešnog formata.", "Greška u učitavanju");
                        wk.ShowDialog();
                        FileName = "";
                        imeFajla.Content = "(AKO JE SALA)";
                        return;
                    }
                    
                }

            }
            catch
            {
                FileName = "";
            }
        }

        private void FormStateChanged(object sender, RoutedEventArgs e)
        {
            if (opis.Text == "" || cena.Text == "")
            {
                dodaj.IsEnabled = false;
                return;
            }

            try
            {
                double a = Double.Parse(cena.Text);
                if (a < 0)
                {
                    dodaj.IsEnabled = false;
                    return;
                }
            }
            catch
            {
                dodaj.IsEnabled = false;
                return;
            }
            dodaj.IsEnabled = true;
        }

    }
}
