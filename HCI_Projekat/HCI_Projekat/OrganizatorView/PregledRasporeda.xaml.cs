using HCI_Projekat.Model;
using Microsoft.Win32;
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
    /// Interaction logic for PregledRasporeda.xaml
    /// </summary>
    public partial class PregledRasporeda : Window, INotifyPropertyChanged
    {
        public Manifestacija Manifestacija { get; set; }

        private ImageSource _image;

        public ImageSource Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged("Image");
                }
            }
        }

        public Dictionary<int, int> BrojMestaZaStolovima { get; set; }
        public Dictionary<int, string> SaleZaStolove { get; set; }

        private static RoutedCommand loadImage = new RoutedCommand();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public PregledRasporeda(Manifestacija current)
        {
            InitializeComponent();
            this.DataContext = this;

            Manifestacija = current;
            BrojMestaZaStolovima = new Dictionary<int, int>();
            SaleZaStolove = new Dictionary<int, string>();

            CommandBinding cb = new CommandBinding(loadImage);
            cb.Executed += new ExecutedRoutedEventHandler(LoadHandler);
            this.CommandBindings.Add(cb);

            //CommandBinding nam omogućava da ako se neka komanda izvrši
            //da prikačimo događaje za razne momente u izvršavanju komande
            //ovdje treba da ide putanja do mape objekta.
            this.DataContext = this;

            List<Gost> l = new List<Gost>();

            using (var db = new DatabaseContext())
            {
                l = (from gos in db.Gosti where gos.Manifestacija.Id == Manifestacija.Id select gos).ToList();
            }

            int brojStolova = 0;
            int flag = 0;

            using (var db = new DatabaseContext())
            {
                Manifestacija manif = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                brojStolova = (from pon in manif.Ponude where pon.Stolovi.Count > 0 select pon.Stolovi.Count).ToArray().Sum();

                foreach (Ponuda p in manif.Ponude)
                {
                    if (flag == 0)
                    {
                        Image = new BitmapImage(new Uri(p.Saradnik.MapaObjekta, UriKind.Relative));
                        flag = 1;
                    }
                    if (p.Stolovi.Count > 0)
                    {
                        foreach (Sto sto in p.Stolovi)
                        {
                            BrojMestaZaStolovima[sto.BrojStola] = sto.BrojOsoba;
                            SaleZaStolove[sto.BrojStola] = p.Opis;
                        }
                    }
                }
            }

            foreach (int i in BrojMestaZaStolovima.Keys)
            {
                ListView novi = new ListView();
                novi.Name = "a_" + (i);
                novi.MinHeight = 200;
                novi.VerticalAlignment = VerticalAlignment.Stretch;
                novi.BorderThickness = new Thickness(10);
                novi.Margin = new Thickness(20, 20, 20, 20);


                var dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                            <WrapPanel>
                                <TextBlock Text=""{Binding ImePrezime}"" FontWeight=""Bold"" FontSize=""22"" />
                            </WrapPanel>

                            </DataTemplate> ";

                var stringReader = new StringReader(dataTemplateString);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                DataTemplate dataTemplate = (DataTemplate)XamlReader.Load(xmlReader);

                novi.ItemTemplate = dataTemplate;

                Label naslov = new Label();
                naslov.Content = "Sto broj: " + i + " - Kapacitet: " + BrojMestaZaStolovima[i] + " - " + SaleZaStolove[i];
                naslov.HorizontalAlignment = HorizontalAlignment.Center;
                naslov.FontSize = 30;

                parentGrid.Children.Add(naslov);
                parentGrid.Children.Add(new Separator());
                parentGrid.Children.Add(novi);
                parentGrid.Children.Add(new Separator());
            }

            foreach (var child in parentGrid.Children)
            {
                if (child is ListView)
                {
                    ListView lv = (ListView)child;
                    int tableNum = Int32.Parse(lv.Name.Split('_')[1]);
                    lv.ItemsSource = new ObservableCollection<Gost>((from s in l where s.BrojStola == tableNum select s));
                }

            }
        }

        private void LoadHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Images|*.jpg;*.png";
                if (openFileDialog.ShowDialog() == true)
                {
                    string url = openFileDialog.FileName;
                    Image = new BitmapImage(new Uri(url, UriKind.Absolute));
                }
            }
            catch
            {

            }
        }

        public void Nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
