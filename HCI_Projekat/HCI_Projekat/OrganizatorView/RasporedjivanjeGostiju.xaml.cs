using HCI_Projekat.Model;
using HCI_Projekat.VlalidationForms;
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
    /// Interaction logic for RasporedjivanjeGostiju.xaml
    /// </summary>
    public partial class RasporedjivanjeGostiju : Window, INotifyPropertyChanged
    {

        #region PropertyChangedNotifier
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        Point startPoint = new Point();
        public ListView InitialSender { get; set; }

        public Manifestacija Manifestacija { get; set; }

        public Dictionary<int, int> BrojMestaZaStolovima { get; set; }
        public Dictionary<int, string> SaleZaStolove { get; set; }

        public RasporedjivanjeGostiju(Manifestacija current)
        {
            InitializeComponent();
            this.DataContext = this;
            Closing += WindowClosing;

            Manifestacija = current;
            BrojMestaZaStolovima = new Dictionary<int, int>();
            SaleZaStolove = new Dictionary<int, string>();

            CommandBinding cb = new CommandBinding(loadImage);
            cb.Executed += new ExecutedRoutedEventHandler(LoadHandler);
            this.CommandBindings.Add(cb);

            //CommandBinding nam omogućava da ako se neka komanda izvrši
            //da prikačimo događaje za razne momente u izvršavanju komande
            //ovdje treba da ide putanja do mape objekta.
            Image = new BitmapImage(new Uri("../../Source/map.jpg", UriKind.Relative));
            this.DataContext = this;

            List<Gost> l = new List<Gost>();

            using (var db = new DatabaseContext())
            {
                l = (from gos in db.Gosti where gos.Manifestacija.Id == Manifestacija.Id select gos).ToList();
            }

            a_0.ItemsSource = new ObservableCollection<Gost>((from g in l where g.BrojStola == 0 select g));

            int brojStolova = 0;

            using (var db = new DatabaseContext())
            {
                Manifestacija manif = (from man in db.Manifestacije where man.Id == Manifestacija.Id select man).FirstOrDefault();
                brojStolova = (from pon in manif.Ponude where pon.Stolovi.Count > 0 select pon.Stolovi.Count).ToArray().Sum();
                
                foreach (Ponuda p in manif.Ponude)
                {
                    if(p.Stolovi.Count > 0)
                    {
                        foreach(Sto sto in p.Stolovi)
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
                novi.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
                novi.DragEnter += ListView_DragEnter;
                novi.Drop += ListView_Drop;
                novi.MouseMove += ListView_MouseMove;
                novi.Name = "a_" + (i);
                novi.AllowDrop = true;
                novi.MinHeight = 200;
                novi.VerticalAlignment = VerticalAlignment.Stretch;
                novi.BorderThickness = new Thickness(10);
                novi.Margin = new Thickness(20, 20, 20, 20);


                var dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                            <WrapPanel>
                                <TextBlock Text=""{Binding ImePrezime}"" FontWeight=""Bold"" FontSize=""20"" />
                            </WrapPanel>

                            </DataTemplate> ";

                var stringReader = new StringReader(dataTemplateString);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                DataTemplate dataTemplate = (DataTemplate)XamlReader.Load(xmlReader);

                novi.ItemTemplate = dataTemplate;

                Label naslov = new Label();
                naslov.Content = "Sto broj: " + i + " - Kapacitet: " + BrojMestaZaStolovima[i] + " - " + SaleZaStolove[i];
                naslov.HorizontalAlignment = HorizontalAlignment.Center;
                naslov.FontSize = 25;

                parentGrid.Children.Add(naslov);
                parentGrid.Children.Add(new Separator());
                parentGrid.Children.Add(novi);
                parentGrid.Children.Add(new Separator());
            }

            foreach (var child in parentGrid.Children)
            {
                if(child is ListView)
                {
                    ListView lv = (ListView)child;
                    int tableNum = Int32.Parse(lv.Name.Split('_')[1]);
                    lv.ItemsSource = new ObservableCollection<Gost>((from s in l where s.BrojStola == tableNum select s));
                }
                
            }

        }

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                InitialSender = listView;
                ListViewItem listViewItem =
                    FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                Gost gost = null;
                try
                {
                    gost = (Gost)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);
                }
                catch (Exception) //nije dobro kliknuto
                {
                    return;
                }


                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", gost);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            ListView target = (ListView)sender;

            if (e.Data.GetDataPresent("myFormat"))
            {
                Gost gost = e.Data.GetData("myFormat") as Gost;
                ObservableCollection<Gost> targetData = (ObservableCollection<Gost>)target.ItemsSource;
                ObservableCollection<Gost> sourceData = (ObservableCollection<Gost>)InitialSender.ItemsSource;

                if(Int32.Parse(target.Name.Split('_')[1]) > 0 && targetData.Count == BrojMestaZaStolovima[Int32.Parse(target.Name.Split('_')[1])])
                {
                    var wk = new OkForm("Kapacitet ovog stola\nje popunjen.", "Nema mesta za stolom");
                    wk.ShowDialog();
                    return;
                }

                sourceData.Remove(gost);
                targetData.Add(gost);
                gost.BrojStola = Int32.Parse(target.Name.Split('_')[1]);

                using(var db = new DatabaseContext())
                {
                    Gost toUpdate = (from gosti in db.Gosti where gosti.Id == gost.Id select gosti).FirstOrDefault();
                    toUpdate.BrojStola = Int32.Parse(target.Name.Split('_')[1]);
                    toUpdate.Manifestacija.PredlozenoZaZavrsavanje = false;
                    Manifestacija.PredlozenoZaZavrsavanje = false;
                    db.SaveChanges();
                }

                target.ItemsSource = targetData;
                InitialSender.ItemsSource = sourceData;
            }
        }

        

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

        private static RoutedCommand loadImage = new RoutedCommand();

        private void LoadHandler(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string url = openFileDialog.FileName;
                Image = new BitmapImage(new Uri(url, UriKind.Absolute));
            }
        }

        private void Nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var wk = new YesNo("Da li ste sigurni\nda zelite da izadjete? \nPromene su\nautomatski sacuvane.", 0, "Potvrda izlaska");
            wk.ShowDialog();

            if (wk.Result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
