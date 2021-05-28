using HCI_Projekat.VlalidationForms;
using Microsoft.Win32;
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
    /// Interaction logic for DodajPonudu.xaml
    /// </summary>
    public partial class DodajPonudu : Window
    {
        private string FileName;
        string retValue = "";
        public DodajPonudu()
        {
            InitializeComponent();
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            retValue= opis.Text + " - " + cena.Text+","+ FileName ;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                FileName = openFileDialog.FileName;

            try
            {
                imeFajla.Content = System.IO.Path.GetFileName(FileName);
                if (System.IO.Path.GetFileName(FileName).Split('.')[1] != "txt")
                {
                    var wk = new OkForm("Niste izabrali txt fajl.", "");
                    wk.ShowDialog();
                    FileName = "";
                }
            }
            catch
            {
                FileName = "";
            }
        }
    }
}
