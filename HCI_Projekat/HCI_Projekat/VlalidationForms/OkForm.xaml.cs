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

        public OkForm(string question, string title, bool hugify = false)
        {
            InitializeComponent();
            QuestionBox.Text = question;
            Title = title;

            if (hugify)
            {
                this.Height = this.Height + 50;
                this.Width = this.Width + 50;
                beli.Height = beli.Height + 50;
                beli.Width = beli.Width + 50;
                QuestionBox.Height = QuestionBox.Height + 50;
                QuestionBox.Width = QuestionBox.Width + 50;
                Ok.FontSize = 22;
                Ok.Height = 40;
                QuestionBox.FontSize = 27;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }
    }
}
