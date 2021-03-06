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
using System.Windows.Threading;

namespace HCI_Projekat.VlalidationForms
{
    /// <summary>
    /// Interaction logic for YesNo.xaml
    /// </summary>
    public partial class YesNo : Window
    {
        public MessageBoxResult Result { get; set; }
        private int Sec { get; set; }
        public YesNo(String question,int s, string t, bool hugify = false)
        {
            InitializeComponent();
            QuestionBox.Text = question;
            Title = t;
            Sec = s;
            Topmost = true;
            if(Sec > 0)
            {
                Yes.IsEnabled = false;
            }
            if (hugify)
            {
                this.Height = this.Height + 50;
                this.Width = this.Width + 50;
                beli.Height = beli.Height + 50;
                beli.Width = beli.Width + 50;
                QuestionBox.Height = QuestionBox.Height + 50;
                QuestionBox.Width = QuestionBox.Width + 50;
                Yes.FontSize = 22;
                Yes.Height = 40;
                No.FontSize = 22;
                No.Height = 40;
                QuestionBox.FontSize = 27;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += dtTicker;
            dt.Start();
        }

        private void dtTicker(object sender, EventArgs e)
        {
            if (Sec > 0)
            {
                Yes.IsEnabled = false;
                string[] tokens = QuestionBox.Text.Split('[');
                string firstPart = tokens[0];
                QuestionBox.Text = firstPart + "[" + Sec + "]";
                Sec -= 1;
            }
            else
            {
                string[] tokens = QuestionBox.Text.Split('[');
                string firstPart = tokens[0];
                QuestionBox.Text = firstPart;
                Yes.IsEnabled = true;
            }
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
