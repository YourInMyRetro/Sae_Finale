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
    /// Logique d'interaction pour Garages.xaml
    /// </summary>
    public partial class Garages : Window
    {
        public Garages()
        {
            InitializeComponent();
            Afficheneige();
        }

        public event Action RetournerAuJeu;

        private void Améliorer_Click(object sender, RoutedEventArgs e)
        {

            if (MainWindow.nbNeiges < 50 * MainWindow.vitesseChasseNeige)
            {
                MessageBox.Show("pas assez de neige");                          //regarde si il ya asser deneige opur amelioration
            }
            else
            {
                MainWindow.vitesseChasseNeige++;
                MainWindow.nbNeiges -= 50 * MainWindow.vitesseChasseNeige;      // soustrait la neige pour ameliorer la vitesse
            }
        }

        private void Afficheneige()
        {
            neigeDispo.Content = "neige " + MainWindow.nbNeiges;                    // affiche la neige sur la page
        }

        private void Jouer_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;           // quand cliquer sur jouer retour au jeu

            this.Close();

        }
    }
}
