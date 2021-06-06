using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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

        Dictionary<string, List<Sto>> Stolovi = new Dictionary<string, List<Sto>>();

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


            if (saradnik.Tip == TipSaradnika.RESTORAN)
            {
                izaberiFajl.Visibility = Visibility.Visible;
                if (imeFajla.Content.ToString() == "IME FAJLA")
                {
                    potvrdi.IsEnabled = false;
                }
            }
            else
            {
                izaberiFajl.Visibility = Visibility.Hidden;
                imeFajla.Visibility = Visibility.Hidden;
            }


            using (var db = new DatabaseContext())
            {
                var p = (from pp in db.Ponude where pp.Saradnik.Id == k.Id select pp).ToList();

                foreach (var item in p)
                {
                    if (item.Obrisana==false) {
                        string tmp = "grid" + btnCounter.ToString();
                        var StackPanelAddApex = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Name=""" + tmp + @""">
                                 <Label Content =""" + item.Opis + @""" Margin = ""5,0,0,0"" Foreground = ""Gray"" FontSize = ""22"" HorizontalAlignment = ""Left"" Width = ""280"" />
                                 <Button Name = ""btn" + btnCounter + @""" Content = ""🗑"" HorizontalAlignment = ""Left"" Cursor = ""Hand"" Width = ""50"" ToolTip = ""Ukloni ponudu"" Margin = ""335,8,0,10.4"" />                      
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
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni \nda želite da obrišete ponudu?", 0, "Obriši ponudu", true);
            wk.ShowDialog();

            if (wk.Result != MessageBoxResult.Yes)
            {
                return;
            }

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
        public string FileName { get; private set; }

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

            if (tip.Text!="")
            {
                if ((TipSaradnika)tip.SelectedItem == TipSaradnika.RESTORAN)
                {
                    izaberiFajl.Visibility = Visibility.Visible;
                    imeFajla.Visibility = Visibility.Visible;
                    if (imeFajla.Content.ToString() == "IME FAJLA" || imeFajla.Content == null)
                    {
                        potvrdi.IsEnabled = false;
                    }
                }
                else
                {
                    izaberiFajl.Visibility = Visibility.Hidden;
                    imeFajla.Visibility = Visibility.Hidden;
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
                var s1 = db.Saradnici.FirstOrDefault(b => b.Id == saradnik.Id);
                if (s1 != null)
                {
                    if(imeFajla.Content != null)
                    {
                        s1.MapaObjekta = imeFajla.Content.ToString();
                    }
                    s1.Naziv = naziv.Text;
                    s1.Specijalizacija = specijalizacija.Text;
                    s1.Adresa = adresa.Text;

                    var pon = (from c in db.Ponude
                               where c.Saradnik.Id == saradnik.Id
                               select c).ToList();
                    //foreach (var item in pon)
                    //{
                    //    item.Obrisana = true;
                    //}

                    foreach (var item in ponude.Children)
                    {
                        if (item is Grid)
                        {
                            string s = "";
                            Grid g = item as Grid;

                            foreach (var decooo in g.Children)
                            {
                                if (decooo is Label)
                                {
                                    Label l = decooo as Label;
                                    s = l.Content.ToString();
                                }
                            }

                            if (s != "PONUDE:")
                            {
                                string opisICena = s.Split(',')[0];

                                if (opisICena.Split('-').Length == 2)
                                {
                                    Ponuda p = new Ponuda(opisICena.Split('-')[0].Trim(), Convert.ToDouble(opisICena.Split('-')[1].Trim()), s1);

                                    foreach (Sto st in Stolovi[opisICena])
                                    {
                                        if (s1.Tip != TipSaradnika.RESTORAN)
                                        {
                                            var wk = new OkForm("Imate ponudu sa stolovima za saradnika koji nije restoran. Uklonite te ponude ili promenite tip saradnika.", "Greška pri čuvanju", true);
                                            wk.ShowDialog();
                                            return;
                                        }
                                        Sto sto1 = new Sto(st.BrojOsoba, st.BrojStola);
                                        db.Stolovi.Add(sto1);
                                        p.Stolovi.Add(sto1);
                                    }

                                    db.Ponude.Add(p);
                                    s1.AddPonuda(p);
                                }

                            }
                        }
                    }
                    
                }
                foreach(Ponuda p in s1.Ponude){
                    p.NazivSaradnika = s1.Naziv;
                }
                db.SaveChanges();
            }
            var dijalog5 = new OkForm("Uspešno ste ažurirali\nsaradnika.", "Uspešno sačuvano", true);
            dijalog5.ShowDialog();
            this.Close();
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {

            bool res = false;
            if (tip.Text == "RESTORAN")
            {
                res = true;
            }

            List<Sto> stolovi = new List<Sto>();
            DodajPonudu dodajPonuduForm = new DodajPonudu(res, stolovi, saradnik);
            
            dodajPonuduForm.ShowDialog();
            string tmp = dodajPonuduForm.Ret;
            if (tmp != "")
            {
                addPonuda(tmp.Split(',')[0]);
                string putanja = tmp.Split(',')[1];
                Stolovi[tmp.Split(',')[0]] = stolovi;
                //using (var db = new DatabaseContext())
                //{
                //    var s1 = db.Saradnici.SingleOrDefault(b => b.Id == saradnik.Id);
                //    Ponuda p = new Ponuda(tmp.Split(',')[0].Split('-')[0],Convert.ToDouble(tmp.Split(',')[0].Split('-')[1]),s1);
                //    if (putanja!="")
                //    {
                //        p.putanjaDoFile = putanja;
                //        using (var reader = new StreamReader(putanja))
                //        {
                //            while (!reader.EndOfStream)
                //            {
                //                var line = reader.ReadLine();
                //                var values = line.Split(',');

                //                Sto sto1 = new Sto(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]));
                //                db.Stolovi.Add(sto1);
                //                p.Stolovi.Add(sto1);
                //            }
                //        }
                //    }
                //    db.Ponude.Add(p);
                //    s1.AddPonuda(p);
                //    db.SaveChanges();
                //}
            }

            //ne diraj ovo i ako ima 10 pametnijih nacina ovo radi
            string tmp1 = adresa.Text;
            adresa.Text = adresa.Text + "a";
            adresa.Text = tmp1;
        }

        void addPonuda(string nazivPonude)
        {
            string tmp = "grid" + btnCounter.ToString();
            var StackPanelAddApex = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Name=""" + tmp + @""">
                                 <Label Content =""" + nazivPonude + @""" Margin = ""5,0,0,0"" Foreground = ""Gray"" FontSize = ""22"" HorizontalAlignment = ""Left"" Width = ""280"" />
                                 <Button Name = ""btn" + btnCounter + @""" Content = ""🗑"" HorizontalAlignment = ""Left"" Cursor = ""Hand"" Width = ""50"" ToolTip = ""Ukloni ponudu"" Margin = ""335,8,0,10.4"" />                      
                            </Grid>";
            btnCounter++;
            var stringReader = new StringReader(StackPanelAddApex);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Grid grid = (Grid)XamlReader.Load(xmlReader);

            Button b = grid.Children[1] as Button;
            b.Click += BtnObrisi_Click;
            ponude.Children.Add(grid);
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
                    FileName = "";
                }
                else if (System.IO.Path.GetFileName(FileName).Split('.')[1] != "jpg" || System.IO.Path.GetFileName(FileName).Split('.')[1] != "png")
                {
                    var wk = new OkForm("Niste izabrali .jpg ili .png fajl.", "Pogrešan format fajla");
                    wk.ShowDialog();
                    FileName = "";
                }
            }
            catch
            {
                FileName = "";
                var wk = new OkForm("Niste izabrali dobar fajl.", "Pogrešan format fajla");
                wk.ShowDialog();
                FileName = "";
            }
        }
    }
}
