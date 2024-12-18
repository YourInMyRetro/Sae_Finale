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

namespace Sae_Chasseneige
{
    /// <summary>
    /// Logique d'interaction pour FenêtreDeDemarrage.xaml
    /// </summary>
    public partial class FenêtreDeDemarrage : Window
    {
        public FenêtreDeDemarrage()
        {
            InitializeComponent();
           // this.WindowState = WindowState.Maximized;
        }
        public static bool lancementReglage, modeChrono;
        public static string nvDifficulté = "facile";

       private void PLacementBouton()
       {
        
        }

        private void Jouer_Click(object sender, RoutedEventArgs e)
        {
            {
                modeChrono = false;
                this.DialogResult = true;
            }
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Reglages_Click(object sender, RoutedEventArgs e)
        {
            lancementReglage = true;

        }

        private void Facile_Click(object sender, RoutedEventArgs e)
        {
            modeChrono = true;
            this.DialogResult = true;
        }

        private void Difficile_Click(object sender, RoutedEventArgs e)
        {
            nvDifficulté = "difficile";
            modeChrono = true;
            this.DialogResult = true;
        }
    }
}

