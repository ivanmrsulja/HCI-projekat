using HCI_Projekat.VlalidationForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public DodajPonudu(bool res)
        {
            InitializeComponent();
            DataContext = this;
            if (res == false)
            {
                izaberiFajl.Visibility = Visibility.Hidden;
                imeFajla.Visibility = Visibility.Hidden;
            }
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
                else if (System.IO.Path.GetFileName(FileName).Split('.')[1] != "txt" || System.IO.Path.GetFileName(FileName).Split('.')[1] != "csv")
                {
                    var wk = new OkForm("Niste izabrali tekstualni fajl.", "");
                    wk.ShowDialog();
                    FileName = "";
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
