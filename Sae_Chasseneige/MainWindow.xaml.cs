using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace Sae_Chasseneige
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer sonDeplacement; // Instance pour gérer le son de déplacement
        public static bool gauche, droite, haut, bas, collision, AfficheVictoire, AfficheDefaite;
        public static double posX;
        public static double posY;
        public static double augmentationX, augmentationY;
        public static double coefficientTaillefenêtreX, coefficientTaillefenêtreY;
        public static int nbNeiges;
        public static int vitesseChasseNeige;
        public static int tempsChrono;
        public static Label chronos = new Label();
        public static MediaPlayer musique;
  

        public MainWindow()
        {
            InitializeComponent();
            FenêtreDeDemarrage fenêtreDémarrage = new FenêtreDeDemarrage();
            fenêtreDémarrage.ShowDialog();
            //    if(Constances.MAP[y, x] == 4)
            if (fenêtreDémarrage.DialogResult == false)
            {
                Application.Current.Shutdown();
                this.Close();
            }
            if (FenêtreDeDemarrage.lancementReglage == true)
            {
                Reglage régle = new Reglage();
                régle.ShowDialog();
            }
            if (FenêtreDeDemarrage.modeChrono == true)
            {
                EmplacementChrono();
            }
            Vitesse();
            EmplacementScoreNeige();
            InitBitMAP();
            InitTimer();
            InitMusique();
        }

        private void Vitesse()
        {
            if (FenêtreDeDemarrage.modeChrono == false)
                vitesseChasseNeige = 3;
            else if (FenêtreDeDemarrage.modeChrono)
                vitesseChasseNeige = 6;
        }

        private void InitMusique()
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/sons/FortniteFinale.mp3"));
            musique.MediaEnded += RelanceMusique;
            musique.Volume =0.5;
            musique.Play();
        }

        private void RelanceMusique(object? sender, EventArgs e)
        {
            if (sender is MediaPlayer player)
            {
                player.Position = TimeSpan.Zero; // Recommence le son depuis le début
                player.Play(); // Relance la lecture
            }
        }




        private void InitBitMAP()
        {
            Constances.chasseNeigeHaut = Constances.CHASSENEIGEHAUT;
            Constances.chasseNeigeBas = Constances.CHASSENEIGEBAS;
            Constances.chasseNeigeGauche = Constances.CHASSENEIGEDROITE;
            Constances.chasseNeigeDroite = Constances.CHASSENEIGEGAUCHE;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            coefficientTaillefenêtreX = this.ActualWidth / 800.0;
            coefficientTaillefenêtreY = this.ActualHeight / 800.0;

            ResetTimer();
            NeigeSimulation(null, EventArgs.Empty);
            ChasseNeigeParDefault();
        }

        private void ResetTimer()
        {
            if (FenêtreDeDemarrage.modeChrono == false)
            {
               Constances.minuterie = new DispatcherTimer();
                Constances.minuterie.Interval = TimeSpan.FromSeconds(5);
                Constances.minuterie.Tick += NeigeSimulation;
                Constances.minuterie.Start();
            }
        }


        private void NeigeSimulation(object? sender, EventArgs e)
        {
            coefficientTaillefenêtreX = this.ActualWidth / 800.0;
            coefficientTaillefenêtreY = this.ActualHeight / 800.0;

            foreach (Rectangle element in Constances.TABLEAU_NEIGE)
            {
                if (element != null)
                {
                    Canvas.Children.Remove(element);
                }
            }


            // Boucle d’abord sur y, puis sur x, pour correspondre à Constances.MAP[y, x]
            for (int y = 0; y < Constances.MAP.GetLength(0); y++)
            {
                for (int x = 0; x < Constances.MAP.GetLength(1); x++)
                {
                    if (Constances.MAP[y, x] == 1)
                    {
                        double tailleX = Constances.TAILLETUILE * coefficientTaillefenêtreX;
                        double tailleY = Constances.TAILLETUILE * coefficientTaillefenêtreY;

                        Rectangle rect = new Rectangle
                        {
                            Width = tailleX,
                            Height = tailleY,
                            Fill = Brushes.White,

                        };

                        Canvas.SetLeft(rect, x * Constances.TAILLETUILE * coefficientTaillefenêtreX);
                        Canvas.SetTop(rect, y * Constances.TAILLETUILE * coefficientTaillefenêtreY);
                        Constances.TABLEAU_NEIGE[x, y] = rect;
                        Canvas.Children.Add(rect);

                    }

                }
            }

        }

        public void EmplacementScoreNeige()
        {
            Constances.neiges.Content = "nbNeige";
            Constances.neiges.FontSize = 25;
            Constances.neiges.Foreground = Brushes.Red;

            // Canvas.SetLeft(neiges, 10*coefficientTaillefenêtreX); // Position horizontale
            //Canvas.SetTop(neiges, 50*coefficientTaillefenêtreY);
            Canvas.Children.Add(Constances.neiges);

        }

        private void InitTimer()
        {
            Constances.minuterie = new DispatcherTimer();
            Constances.minuterie.Interval = TimeSpan.FromMilliseconds(16);
            Constances.minuterie.Tick += Deplacement;
            Constances.minuterie.Tick += StockNeige;
            Constances.minuterie.Start();
            if (FenêtreDeDemarrage.modeChrono == true)
            {
                Constances.minuterie = new DispatcherTimer();
                Constances.minuterie.Interval = TimeSpan.FromSeconds(1);
                Constances.minuterie.Tick += Chronomètre;
                Constances.minuterie.Tick += Victoire;
                Constances.minuterie.Start();

            }
        }

        private void Victoire(object? sender, EventArgs e)
        {
            if (tempsChrono < 0)
            {
                Console.WriteLine("chronos inferieur a 0");
                Defaite fenêtreDefaite = new Defaite();
                fenêtreDefaite.ShowDialog();
                Constances.minuterie.Stop();
            }
            else if (nbNeiges >= Constances.NOMBREDENEIGE)
            {
                AfficheVictoire = true;
                Console.WriteLine("Victoire atteinte !");
                Victoires fenêtreVictoire = new Victoires();

                // Bloque l'exécution ici jusqu'à fermeture de la fenêtre.
                bool? resultat = fenêtreVictoire.ShowDialog();

                if (resultat == true)
                {
                    Console.WriteLine("L'utilisateur a cliqué sur Continuer !");
                }

                Constances.minuterie.Stop();
            }
        }

        private void Chronomètre(object? sender, EventArgs e)
        {
            int minutes = tempsChrono / 60;
            int seconde = tempsChrono % 60;

            tempsChrono += -1;
            chronos.Content = "Temps : " + minutes + " : " + seconde;
            Canvas.SetLeft(chronos, 0 * coefficientTaillefenêtreX); // Position horizontale
            Canvas.SetTop(chronos, 20 * coefficientTaillefenêtreY);

        }


        public void EmplacementChrono()
        {
            if (FenêtreDeDemarrage.nvDifficulté == "facile")
            {
                tempsChrono = 60 * 5;

            }
            if (FenêtreDeDemarrage.nvDifficulté == "difficile")
            {
                tempsChrono = 60 * 3;
            }

            chronos.Content = "Temps : ";
            chronos.FontSize = 25;
            chronos.Foreground = Brushes.Red;


            Canvas.Children.Add(chronos);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    droite = true;
                    break;
                case Key.Left:
                    gauche = true;
                    break;
                case Key.Up:
                    haut = true;
                    break;
                case Key.Down:
                    bas = true;
                    break;
                default:    // ignore les autres touches
                    break;
            }
            if (droite || gauche || haut || bas)
            {
                // Jouer le son de déplacement
                if (sonDeplacement == null) // Crée l'instance si elle n'existe pas
                {
                    sonDeplacement = new MediaPlayer();
                    sonDeplacement.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/sons/v8.mp3"));
                    sonDeplacement.Volume = 0.2; // ajustement du volume 
                    sonDeplacement.MediaEnded += RelanceMusique;// appel à la méthode relance pour pas que le son du v8 s'arrête après plusieurs secondes 
                }

                // Si le son n'est pas en train de jouer, lance-le
                if (sonDeplacement.Position == TimeSpan.Zero || sonDeplacement.HasAudio == false)
                {
                    sonDeplacement.Play();
                }
            }
        }   

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    droite = false;
                    break;
                case Key.Left:
                    gauche = false;
                    break;
                case Key.Up:
                    haut = false;
                    break;
                case Key.Down:
                    bas = false;
                    break;
                default:    // ignore les autres touches
                    break;

            }
            if (!droite && !gauche && !haut && !bas)
            {
                // Arrêter le son de déplacement
                if (sonDeplacement != null)
                {
                    sonDeplacement.Stop();
                    sonDeplacement.Position = TimeSpan.Zero; // Réinitialise le son
                }
            }
        }

        private void Deplacement(object? sender, EventArgs e)
        {
            // Récupération de la position actuelle
            double nouvellePosX = Canvas.GetLeft(Constances.ChasseNeige);
            double nouvellePosY = Canvas.GetTop(Constances.ChasseNeige);

            // Calcul des nouvelles positions en fonction des directions
            if (haut)
            {
                nouvellePosY -= vitesseChasseNeige;
                Constances.ChasseNeige.Source = Constances.chasseNeigeHaut;
            }
            if (bas)
            {
                nouvellePosY += vitesseChasseNeige;
                Constances.ChasseNeige.Source = Constances.chasseNeigeBas;
            }
            if (gauche)
            {
                nouvellePosX -= vitesseChasseNeige;
                Constances.ChasseNeige.Source = Constances.chasseNeigeGauche;
            }
            if (droite)
            {
                nouvellePosX += vitesseChasseNeige;
                Constances.ChasseNeige.Source = Constances.chasseNeigeDroite;
            }


            if (!Collision(nouvellePosX, nouvellePosY))     // Vérifie si il ya des colision avnt de positioner le véhicule
            {
                posX = nouvellePosX;  // mise à jour de la position car pas de collision
                posY = nouvellePosY;

                Canvas.SetLeft(Constances.ChasseNeige, posX);
                Canvas.SetTop(Constances.ChasseNeige, posY);

                
            }
            else
            {
                // Collision détectée, déplacement bloqué
                Console.WriteLine("Collision détectée, déplacement bloqué.");
            }
        }

        private void ChasseNeigeParDefault()
        {
            // Recherche d'une position valide
            int startX = 5, startY = 5;
            while (Constances.MAP[startY, startX] != 1)
            {
                startX++;
                if (startX >= Constances.MAP.GetLength(1))
                {
                    startX = 0;
                    startY++;
                }
            }


            posX = startX * Constances.TAILLETUILE * coefficientTaillefenêtreX;
            posY = startY * Constances.TAILLETUILE * coefficientTaillefenêtreY;     // Position initiale en pixels

            // Taille ajustée pour correspondre à la hitbox
            double largeurCorrection = 17 / Constances.TAILLETUILE; // Conversion de la correction en unités de tuiles
            double longueurCorrection = 35 / Constances.TAILLETUILE;
            Constances.ChasseNeige.Width = Constances.TAILLETUILE * (Constances.LARGEURCHASSENEIGE - largeurCorrection) * coefficientTaillefenêtreX;
            Constances.ChasseNeige.Height = Constances.TAILLETUILE * (Constances.LONGUEURCHASSENEIGE - longueurCorrection) * coefficientTaillefenêtreY;

            // Placement initial
            Canvas.SetLeft(Constances.ChasseNeige, posX);
            Canvas.SetTop(Constances.ChasseNeige, posY);

            // Assignation de la texture et ajout au canvas
            Constances.ChasseNeige.Source = Constances.chasseNeigeSource;
            Canvas.Children.Add(Constances.ChasseNeige);
        }

        private void StockNeige(object? sender, EventArgs e)
        {
            Constances.neiges.Content = "Neiges " + nbNeiges;

        }
     


        private bool Collision(double posX, double posY)
        {
            double largeurChasseNeige = (Constances.TAILLETUILE * Constances.LARGEURCHASSENEIGE - 64) * coefficientTaillefenêtreX;
            double longueurChasseNeige = (Constances.TAILLETUILE * Constances.LONGUEURCHASSENEIGE - 82) * coefficientTaillefenêtreY;

            double hitboxX = posX + (23 * coefficientTaillefenêtreX);
            double hitboxY = posY + (25 * coefficientTaillefenêtreY);

            int debutX = Math.Clamp((int)(hitboxX / (Constances.TAILLETUILE * coefficientTaillefenêtreX)), 0, Constances.MAP.GetLength(1) - 1);
            int debutY = Math.Clamp((int)(hitboxY / (Constances.TAILLETUILE * coefficientTaillefenêtreY)), 0, Constances.MAP.GetLength(0) - 1);
            int finX = Math.Clamp((int)((hitboxX + largeurChasseNeige) / (Constances.TAILLETUILE * coefficientTaillefenêtreX)), 0, Constances.MAP.GetLength(1) - 1);
            int finY = Math.Clamp((int)((hitboxY + longueurChasseNeige) / (Constances.TAILLETUILE * coefficientTaillefenêtreY)), 0, Constances.MAP.GetLength(0) - 1);

            for (int y = debutY; y <= finY; y++)
            {
                for (int x = debutX; x <= finX; x++)
                {

                    if (Constances.MAP[y, x] == 0) // Collision détectée
                    {
                        return true;
                    }
                    else if (Constances.MAP[y, x] == 1) // Si neige
                    {
                        Rectangle rect = Constances.TABLEAU_NEIGE[x, y];

                        if (rect != null)
                        {
                            Canvas.Children.Remove(rect);
                            nbNeiges++;
                            MediaPlayer bruitNeige = new MediaPlayer();
                            bruitNeige.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/sons/neige.mp3"));
                            bruitNeige.Volume = 0.09;
                            bruitNeige.Play();
                        }

                        Constances.TABLEAU_NEIGE[x, y] = null;
                    }
                    else if (Constances.MAP[y, x] == 4) // Garage
                    {
                        if (FenêtreDeDemarrage.modeChrono == false)
                        {
                            gauche = droite = haut = bas = false; // Réinitialisation des directions

                            posX += Constances.TAILLETUILE * coefficientTaillefenêtreX;
                            Canvas.SetLeft(Constances.ChasseNeige, posX); // Mise à jour de la position

                            Garages fenetreGarage = new Garages();

                            fenetreGarage.ShowDialog(); // Ouvre la fenêtre du garage

                            return true;

                        }

                    }
                }

            }
            return false; // Par défaut, aucune collision
        }

    }


}
