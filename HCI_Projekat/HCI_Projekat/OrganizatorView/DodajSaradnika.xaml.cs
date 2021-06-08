using Apex.Controls;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace HCI_Projekat.OrganizatorView
{
    /// <summary>
    /// Interaction logic for DodajSaradnika.xaml
    /// </summary>
    public partial class DodajSaradnika : Window, INotifyPropertyChanged
    {
        private List<TipSaradnika> _tip;
        public event PropertyChangedEventHandler PropertyChanged;
        private string FileName;
        int btnCounter = 1;
        string putanjaDoFajla = "";
        bool isOrg;
        Dictionary<string, List<Sto>> Stolovi = new Dictionary<string, List<Sto>>();

        public DodajSaradnika(bool isOrg = false)
        {
            this.isOrg = isOrg;
            InitializeComponent();
            this.DataContext = this;

            Tip = new List<TipSaradnika> { TipSaradnika.RESTORAN, TipSaradnika.FOTOGRAF, TipSaradnika.KETERING, TipSaradnika.DEKORACIJE, TipSaradnika.MUZIKA };
            izaberiFajl.Visibility = Visibility.Hidden;
            imeFajla.Visibility = Visibility.Hidden;
        }

        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
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

        private void FormStateChanged(object sender, EventArgs e)
        {

            if (tip.Text.Trim() == "" || naziv.Text.Trim() == "" || adresa.Text.Trim() == "" || specijalizacija.Text.Trim() == "")
                potvrdi.IsEnabled = false;
            else
            {
                potvrdi.IsEnabled = true;
            }

            if (tip.Text!="") {
                if ((TipSaradnika)tip.SelectedItem == TipSaradnika.RESTORAN)
                {
                    izaberiFajl.Visibility = Visibility.Visible;
                    imeFajla.Visibility = Visibility.Visible;
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
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void imeFajla_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                if (openFileDialog.ShowDialog() == true)
                FileName = openFileDialog.FileName;
           
                imeFajla.Content = System.IO.Path.GetFileName(FileName);
                if (System.IO.Path.GetFileName(FileName) == null)
                {
                    FileName = "";
                    var wk = new OkForm("Niste izabrali dobar fajl.", "Pogrešan format fajla");
                    wk.ShowDialog();
                    imeFajla.Content = "IME FAJLA";
                }
                else if (System.IO.Path.GetFileName(FileName).Split('.')[1] != "jpg" && System.IO.Path.GetFileName(FileName).Split('.')[1] != "png")
                {
                    var wk = new OkForm("Niste izabrali .jpg ili .png fajl.", "Pogrešan format fajla");
                    wk.ShowDialog();
                    FileName = "";
                    imeFajla.Content = "IME FAJLA";
                }
                             
            }
            catch
            {
                FileName = "";
                imeFajla.Content = "IME FAJLA";
            }

            //ne diraj ovo 
            string tmp = adresa.Text;
            adresa.Text = adresa.Text + "a";
            adresa.Text = tmp;

        }

        void addPonuda(string nazivPonude)
        {
            string tmp= "grid"+btnCounter.ToString();
            var StackPanelAddApex = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Name="""+tmp+@""">
                                 <Label Content =""" + nazivPonude + @""" Margin = ""5,0,0,0"" Foreground = ""Gray"" FontSize = ""22"" HorizontalAlignment = ""Left"" Width = ""280"" />
                                 <Button Name = ""btn" + btnCounter + @""" Content = ""🗑"" HorizontalAlignment = ""Left"" Cursor = ""Hand"" Width = ""50"" ToolTip = ""Ukloni ponudu"" Margin = ""335,8,0,10.4"" />                      
                            </Grid>";
            btnCounter++;
            var stringReader = new StringReader(StackPanelAddApex);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Grid grid = (Grid)XamlReader.Load(xmlReader);

            Button b=grid.Children[1] as Button;
            b.Click += BtnObrisi_Click;
            ponude.Children.Add(grid);
        }
        
        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            bool res = false;
            if (tip.Text == "RESTORAN")
            {
                res = true;   
            }

            List<Sto> stolovi = new List<Sto>();
            DodajPonudu dodajPonuduForm = new DodajPonudu(res, stolovi, null);

            dodajPonuduForm.ShowDialog();
            string tmp = dodajPonuduForm.Ret;
            if (tmp != "")
            {
                addPonuda(tmp.Split(',')[0]);
                putanjaDoFajla = tmp.Split(',')[1];
                Stolovi[tmp.Split(',')[0]] = stolovi;
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tmp = btn.Name.ToString();
            tmp=tmp.Remove(0, 3);
            Grid brisanje = null;
            foreach (var item in ponude.Children)
            {
                if(item is Grid)
                {
                    Grid g = item as Grid;
                    if(g.Name=="grid"+tmp)
                    {
                        brisanje = g;
                    }
                }

            }
            if(brisanje!=null)
                ponude.Children.Remove(brisanje);
        }

        TipSaradnika tipSaradnika(string t)
        {
            string tipUpper = t.ToUpper();
            if(tipUpper == "RESTORAN")
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
            List<string> keepingTrack = new List<string>();
            using (var db = new DatabaseContext())
            {
                string destFileName = "";
                
                if (FileName != "" && FileName != null)
                {
                    Console.WriteLine(FileName);
                    Console.WriteLine("../../Images/" + System.IO.Path.GetFileName(FileName));
                    int localId = (from sar in db.Saradnici select sar.Id).Max() + 1;
                    destFileName = "../../Images/map" + localId + "." + System.IO.Path.GetFileName(FileName).Split('.')[1];
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                    }
                    File.Copy(FileName, destFileName);
                }
                

                Saradnik s1 = new Saradnik(naziv.Text, adresa.Text, tipSaradnika(tip.Text), specijalizacija.Text, destFileName);
                db.Saradnici.Add(s1);

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
                            Ponuda p = new Ponuda(opisICena.Split('-')[0].Trim(), Convert.ToDouble(opisICena.Split('-')[1].Trim()), s1);
                            foreach(string op in keepingTrack)
                            {
                                if(op == opisICena.Split('-')[0])
                                {
                                    var wk = new OkForm("Dve ponude kod istog saradnika ne mogu imati isti opis.", "Dupliran opis", true);
                                    wk.ShowDialog();
                                    return;
                                }
                            }
                            keepingTrack.Add(opisICena.Split('-')[0]);

                            foreach(Sto st in Stolovi[opisICena])
                            {   
                                if(s1.Tip != TipSaradnika.RESTORAN)
                                {
                                    var wk = new OkForm("Imate ponudu sa stolovima za saradnika koji nije restoran. Uklonite te ponude ili promenite tip.", "Greška pri čuvanju", true);
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
                this.Close();
                db.SaveChanges();
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DependencyObject)
            {
                string str = HelpProvider.GetHelpKey((DependencyObject)sender);
                if (isOrg)
                {
                    HelpProvider.ShowHelp(str, this, isOrg);
                }
                else
                {
                    HelpProvider.ShowHelp(str, this);
                }

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelp("HelpDodajSaradnika", this , isOrg);
        }
    }
}
