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

namespace HCI_Projekat.VlalidationForms
{
    /// <summary>
    /// Interaction logic for Ok.xaml
    /// </summary>
    public partial class OkForm : Window
    {
        public MessageBoxResult Result { get; set; }

        public OkForm(string question)
        {
            InitializeComponent();
            QuestionBox.Text = question;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }
    }
}
