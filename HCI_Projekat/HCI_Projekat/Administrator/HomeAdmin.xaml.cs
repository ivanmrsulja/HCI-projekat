﻿using System;
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

namespace HCI_Projekat.Administrator
{
    /// <summary>
    /// Interaction logic for HomeAdmin.xaml
    /// </summary>
    public partial class HomeAdmin : Window
    {
        public Window ParentScreen { get; set; }
        public HomeAdmin(Window p)
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