using HCI_Projekat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for PregledStareManifestacije.xaml
    /// </summary>
    public partial class PregledStareManifestacije : Window
    {

        public Manifestacija Manifestacija { get; set; }

        public ObservableCollection<Ponuda> Ponude { get; set; }

        public PregledStareManifestacije(Manifestacija current)
        {
            InitializeComponent();
            DataContext = this;
            using (var db = new DatabaseContext())
            {
                Manifestacija = (from man in db.Manifestacije where current.Id == man.Id select man).FirstOrDefault();
                Ponude = new ObservableCollection<Ponuda>((from man in db.Manifestacije where man.Id == Manifestacija.Id select man.Ponude).FirstOrDefault().ToList());
            }
        }

        public void Pogledaj_Click(object sender, RoutedEventArgs e)
        {
            var w = new PregledRasporeda(Manifestacija);
            w.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
