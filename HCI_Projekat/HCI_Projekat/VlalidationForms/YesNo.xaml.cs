using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for YesNo.xaml
    /// </summary>
    public partial class YesNo : Window
    {
        public MessageBoxResult Result { get; set; }
        int sec { get; set; }
        public YesNo(String question,int s)
        {
            InitializeComponent();
            QuestionBox.Text = question;
            sec = s;
            Loaded += Window_Loaded;
            Topmost = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //for (int i = sec; i > 0; i--)
            //{
            //    Yes.Content = "(" + i.ToString() + ")" + "sec";
            //    Thread.Sleep(1000);
            //}
            //Yes.Content = "DA";
            
        }

    private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            this.Close();
        }

       
    }
}
