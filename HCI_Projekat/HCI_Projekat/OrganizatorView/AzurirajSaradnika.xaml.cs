using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace HCI_Projekat.OrganizatorView
{
    /// <summary>
    /// Interaction logic for AzurirajSaradnika.xaml
    /// </summary>
    public partial class AzurirajSaradnika : Window
    {
        private List<TipSaradnika> _tip;
        private Saradnik saradnik;
        private string putanjaDoFajla="";
        int btnCounter = 1;

        public AzurirajSaradnika(Saradnik k)
        {
            InitializeComponent();
            DataContext = this;
            saradnik = k;
            Tip = new List<TipSaradnika> { TipSaradnika.RESTORAN, TipSaradnika.FOTOGRAF, TipSaradnika.KETERING, TipSaradnika.DEKORACIJE, TipSaradnika.MUZIKA };
            klijent = k;
            naziv.Text = k.Naziv;
            adresa.Text = k.Adresa;
            tip.SelectedIndex = retTip(k.Tip.ToString());
            specijalizacija.Text = k.Specijalizacija;
            imeFajla.Content = k.MapaObjekta;
            

            using (var db = new DatabaseContext())
            {
                var p = (from pp in db.Ponude where pp.Saradnik.Id == k.Id select pp).ToList();

                foreach (var item in p)
                {
                    string tmp = "grid" + btnCounter.ToString();
                    var StackPanelAddApex = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Name=""" + tmp + @""">
                                 <Label Content =""" + item.Opis + @""" Margin = ""10,0,0,0"" Foreground = ""Gray"" FontSize = ""18"" HorizontalAlignment = ""Left"" Width = ""280"" />
                                 <Button Name = ""btn" + btnCounter + @""" Content = ""X"" HorizontalAlignment = ""Left"" Cursor = ""Hand"" Width = ""45"" Margin = ""280,8,0,10.4"" />                      
                            </Grid>";
                    btnCounter++;
                    var stringReader = new StringReader(StackPanelAddApex);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Grid grid = (Grid)XamlReader.Load(xmlReader);

                    Button b = grid.Children[1] as Button;
                    b.Click += BtnObrisi_Click;
                    ponude.Children.Add(grid);
                }
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tmp = btn.Name.ToString();
            tmp = tmp.Remove(0, 3);
            Grid brisanje = null;
            foreach (var item in ponude.Children)
            {
                if (item is Grid)
                {
                    Grid g = item as Grid;
                    if (g.Name == "grid" + tmp)
                    {
                        brisanje = g;
                    }
                }

            }
            if (brisanje != null)
                ponude.Children.Remove(brisanje);
        }

        public List<TipSaradnika> Tip
        {
            get
            {
                return _tip;
            }
            set
            {
                _tip = value;
                OnPropertyChanged("Tip");
            }
        }

        public Saradnik klijent { get; set; }
        public object Sender { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        int retTip(string t)
        {
            if (t == "RESTORAN")
                return 0;
            else if (t == "FOTOGRAF")
                return 1;
            else if (t == "KETERING")
                return 2;
            else if (t == "DEKORACIJE")
                return 3;
            else
                return 4;
        }

        private void FormStateChanged(object sender, RoutedEventArgs e)
        {

            if (tip.Text.Trim() == "" || naziv.Text.Trim() == "" || adresa.Text.Trim() == "" || specijalizacija.Text.Trim() == "")
                potvrdi.IsEnabled = false;
            else
            {
                potvrdi.IsEnabled = true;
            }

            if (tip.Text == "RESTORAN")
            {
                izaberiFajl.IsEnabled = true;
                if (imeFajla.Content.ToString() == "IME FAJLA")
                {
                    potvrdi.IsEnabled = false;
                }
            }
        }

        private void odustani_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        TipSaradnika ts(string t)
        {
            string tipUpper = t.ToUpper();
            if (tipUpper == "RESTORAN")
                return TipSaradnika.RESTORAN;
            else if (tipUpper == "MUZIKA")
                return TipSaradnika.MUZIKA;
            else if (tipUpper == "KETERING")
                return TipSaradnika.KETERING;
            else if (tipUpper == "FOTOGRAF")
                return TipSaradnika.FOTOGRAF;

            return TipSaradnika.DEKORACIJE;
        }

        private void Potvrdi_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DatabaseContext())
            {
                var s1 = db.Saradnici.SingleOrDefault(b => b.Id == saradnik.Id);
                if (s1 != null)
                {
                    s1.MapaObjekta = imeFajla.Content.ToString();
                    s1.Naziv = naziv.Text;
                    s1.Specijalizacija = specijalizacija.Text;
                    s1.Tip = ts(tip.Text);
                    s1.Adresa = adresa.Text;

                    //s1.Ponude.Clear();
                    //foreach (var item in ponude.Children)
                    //{
                    //    if (item is Grid)
                    //    {
                    //        string s = "";
                    //        Grid g = item as Grid;

                    //        foreach (var decooo in g.Children)
                    //        {
                    //            if (decooo is Label)
                    //            {
                    //                Label l = decooo as Label;
                    //                s = l.Content.ToString();
                    //            }
                    //        }
                    //        if (s != "PONUDE:")
                    //        {
                    //            string opisICena = s.Split(',')[0];
                    //            var pon = db.Ponude.SingleOrDefault(b => b.Opis == opisICena.Split('-')[0] && b.Cena == Convert.ToDouble(opisICena.Split('-')[1]));
                                

                    //            foreach (Sto sto in pon.Stolovi)
                    //            {
                    //                db.Stolovi.Add(sto);
                                    
                    //            }

                    //            s1.AddPonuda(pon);
                    //            db.SaveChanges();
                                
                    //        }
                    //    }
                    //}
                    db.SaveChanges();
                    saradnik = s1 as Saradnik;
                }
            }
            var dijalog5 = new OkForm("Uspesno ste azurirali\nprofil.", "Uspesno sacuvano");
            dijalog5.ShowDialog();
            this.Close();
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            DodajPonudu dodajPonuduForm = new DodajPonudu();
            dodajPonuduForm.ShowDialog();
            string tmp = dodajPonuduForm.Ret;
            if (tmp != "")
            {
                addPonuda(tmp.Split(',')[0]);
                putanjaDoFajla = tmp.Split(',')[1];
            }
        }

        void addPonuda(string nazivPonude)
        {
            string tmp = "grid" + btnCounter.ToString();
            var StackPanelAddApex = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Name=""" + tmp + @""">
                                 <Label Content =""" + nazivPonude + @""" Margin = ""10,0,0,0"" Foreground = ""Gray"" FontSize = ""18"" HorizontalAlignment = ""Left"" Width = ""280"" />
                                 <Button Name = ""btn" + btnCounter + @""" Content = ""X"" HorizontalAlignment = ""Left"" Cursor = ""Hand"" Width = ""45"" Margin = ""280,8,0,10.4"" />                      
                            </Grid>";
            btnCounter++;
            var stringReader = new StringReader(StackPanelAddApex);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Grid grid = (Grid)XamlReader.Load(xmlReader);

            Button b = grid.Children[1] as Button;
            b.Click += BtnObrisi_Click;
            ponude.Children.Add(grid);
        }
        
    }
}
