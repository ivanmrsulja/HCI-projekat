using HCI_Projekat.Model;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for PregledNedodeljeneManifestacije.xaml
    /// </summary>
    public partial class PregledNedodeljeneManifestacije : Window
    {
        public Manifestacija Manifestacija { get; set; }

        public PregledNedodeljeneManifestacije(Manifestacija current)
        {
            InitializeComponent();
            DataContext = this;
            using (var db = new DatabaseContext())
            {
                Manifestacija = (from man in db.Manifestacije where current.Id == man.Id select man).FirstOrDefault();
            }
        }

        public void Nazad_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
