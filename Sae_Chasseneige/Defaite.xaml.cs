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

namespace Sae_Chasseneige
{
    /// <summary>
    /// Logique d'interaction pour Defaite.xaml
    /// </summary>
    public partial class Defaite : Window
    {
        public Defaite()
        {
            InitializeComponent();
        }

        private void Continuer_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();// ferme lppli
            this.Close();
        }
    }
}
