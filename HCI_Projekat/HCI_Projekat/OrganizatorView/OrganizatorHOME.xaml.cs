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
    /// Interaction logic for OrganizatorHOME.xaml
    /// </summary>
    public partial class OrganizatorHOME : Window
    {
        public Window ParentScreen { get; set; }
        public OrganizatorHOME(Window p)
        {
            InitializeComponent();
            ParentScreen = p;
            ParentScreen.Hide();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentScreen.Show();
        }
    }
}
