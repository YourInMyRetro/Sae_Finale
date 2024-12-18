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
        private MediaPlayer sonDeplacement; // gérer le son du déplacement
        public static bool gauche, droite, haut, bas; // gérer les sens
        public static bool collision, AfficheVictoire, AfficheDefaite; // bolléen pour vérifier les collision et 2 autres bolléen qui s'active en condition de victoire ou de defaite
        public static double posX; 
        public static double posY;
        public static double augmentationX, augmentationY;
        public static double coefficientTaillefenêtreX, coefficientTaillefenêtreY;
        public static int nbNeiges; // nombre de neige récupérer
        public static int vitesseChasseNeige; // vitesse du Chasse Neige
        public static int tempsChrono; // Temps de base du chrono qui s'active uniquement en modeChrono(mode Facile ou Difficile)
        public static Label chronos = new Label(); // Chronomètre actuel qui est affiché
        public static MediaPlayer musique;// gérer la musique de fond
  

        public MainWindow()
        {
            InitializeComponent();
            FenêtreDeDemarrage fenêtreDémarrage = new FenêtreDeDemarrage(); 
            fenêtreDémarrage.ShowDialog(); // affiche la fenêtre du début 
            //    if(Constances.MAP[y, x] == 4)
            if (fenêtreDémarrage.DialogResult == false)
            {
                Application.Current.Shutdown();// ferme l'appli
                this.Close(); // fermer la fenêtre
            }
            if (FenêtreDeDemarrage.lancementReglage == true)
            {
                Reglage régle = new Reglage();
                régle.ShowDialog();
            }
            if (FenêtreDeDemarrage.modeChrono == true)
            {
                EmplacementChrono(); // si mode facile ou difficile alors, afficher le chrono
            }
            Vitesse();
            EmplacementScoreNeige();
            InitBitMAP();
            InitTimer();
            InitMusique();
        }

        private void Vitesse()
        {
            if (FenêtreDeDemarrage.modeChrono == false) // si en mode farming, la vitesse est de 3
                vitesseChasseNeige = 3;
            else if (FenêtreDeDemarrage.modeChrono) // si en mode facile ou difficile, la vitesse est de 6
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
            Constances.chasseNeigeDroite = Constances.CHASSENEIGEGAUCHE;  // Image du chasse neige en fonction de ca position
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            coefficientTaillefenêtreX = this.ActualWidth / 800.0;
            coefficientTaillefenêtreY = this.ActualHeight / 800.0;  // dès que la fenêtre a finis de ce lancer

            ResetTimer();
            NeigeSimulation(null, EventArgs.Empty);  // Pause la neige sur la map
            ChasseNeigeParDefault(); 
        }

        private void ResetTimer()
        {
            if (FenêtreDeDemarrage.modeChrono == false)
            {
                Constances.minuterie = new DispatcherTimer();                // Active le reset de la neige en mode farming (toute les 1 minute)
                Constances.minuterie.Interval = TimeSpan.FromMinutes(1);
                Constances.minuterie.Tick += NeigeSimulation;
                Constances.minuterie.Start();
            }
        }

        private void NeigeSimulation(object? sender, EventArgs e)
        {
            coefficientTaillefenêtreX = this.ActualWidth / 800.0;           // calcul du coefficient d'agrandissement de la page 
            coefficientTaillefenêtreY = this.ActualHeight / 800.0;

            foreach (Rectangle element in Constances.TABLEAU_NEIGE)
            {
                if (element != null)
                {
                    Canvas.Children.Remove(element);//retire lancienne neige si il ya un element dans le tableau de la neige, elle est supprimé
                }
            }

            // Boucle d’abord sur y, puis sur x, pour correspondre à Constances.MAP[y, x]
            for (int y = 0; y < Constances.MAP.GetLength(0); y++)
            {
                for (int x = 0; x < Constances.MAP.GetLength(1); x++)
                {
                    if (Constances.MAP[y, x] == 1)          // regarde dans chaque élement du tableau MAP
                    {
                        double tailleX = Constances.TAILLETUILE * coefficientTaillefenêtreX;      // taille x et taille y des pixels      
                        double tailleY = Constances.TAILLETUILE * coefficientTaillefenêtreY;

                        Rectangle rect = new Rectangle
                        {
                            Width = tailleX,
                            Height = tailleY,
                            Fill = Brushes.White, // simule de la neige en réalisant un carré blanc, en dessinant des carrés pour la neige de la tailles x et y calculé precedemment

                        };

                        Canvas.SetLeft(rect, x * Constances.TAILLETUILE * coefficientTaillefenêtreX);       // Positionne la neige
                        Canvas.SetTop(rect, y * Constances.TAILLETUILE * coefficientTaillefenêtreY);
                        Constances.TABLEAU_NEIGE[x, y] = rect; // met un rectangle dans tableau neige au position x et y 
                        Canvas.Children.Add(rect); // ajoute le dessin du rectangle sur le Canvas
                    }
                }
            }
        }

        public void EmplacementScoreNeige()
        {
            Constances.neiges.Content = "nbNeige";
            Constances.neiges.FontSize = 25; // met le texte en gras
            Constances.neiges.Foreground = Brushes.Red; // met de couleur rouge le texte de neige

     
            Canvas.Children.Add(Constances.neiges); // affiche le nombre de neige récolter

        }

        private void InitTimer()
        {
            Constances.minuterie = new DispatcherTimer();
            Constances.minuterie.Interval = TimeSpan.FromMilliseconds(16); 
            Constances.minuterie.Tick += Deplacement;                           // timer qui actualise les déplacement et le stock de neige
            Constances.minuterie.Tick += StockNeige;
            Constances.minuterie.Start();
            if (FenêtreDeDemarrage.modeChrono == true)
            {
                Constances.minuterie = new DispatcherTimer();
                Constances.minuterie.Interval = TimeSpan.FromSeconds(1);        // si lancement en mode Facile ou Diffile actualise le chronomètre et actualise la méthode Victoire
                Constances.minuterie.Tick += Chronomètre;
                Constances.minuterie.Tick += Victoire;
                Constances.minuterie.Start();

            }
        }

        private void Victoire(object? sender, EventArgs e)
        {
            if (tempsChrono < 0)                                        // Si le temps du chrono est inférieur à 0 alors la fenetre perdu s'ouvre
            {
                MessageBox.Show("Défaite !");
                this.Close();

            }
            else if (nbNeiges >= Constances.NOMBREDENEIGE)             // si toute la neige est récupere alors la fenêtre gagner s'ouvre
            {
                AfficheVictoire = true;
                MessageBox.Show("Victoire !");
                this.Close();
                

            }
        }

        private void Chronomètre(object? sender, EventArgs e)
        {
            int minutes = tempsChrono / 60;                             
            int seconde = tempsChrono % 60;             

            tempsChrono += -1;
            chronos.Content = "Temps : " + minutes + " : " + seconde;   // le temps en minute
            Canvas.SetLeft(chronos, 0 * coefficientTaillefenêtreX); // Position horizontale et verticale
            Canvas.SetTop(chronos, 20 * coefficientTaillefenêtreY);

        }


        public void EmplacementChrono()
        {
            if (FenêtreDeDemarrage.nvDifficulté == "facile")
            {
                tempsChrono = 60 * 5;                               // augmente temps  du Chrono est augmenté a 5 minutes en mode facile

            }
            if (FenêtreDeDemarrage.nvDifficulté == "difficile")
            {   
                tempsChrono = 60 * 3;                               //le temps est baissé car le mode est difficle 3 minutes en mode difficle
            }

            chronos.Content = "Temps : ";
            chronos.FontSize = 25;
            chronos.Foreground = Brushes.Red;                         // couleur rouge pour le temps


            Canvas.Children.Add(chronos);                               // Affiche le label du chrono sur le jeu
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
                // joue le son de déplacement
                if (sonDeplacement == null) // crée l'instance si elle n'existe pas
                {
                    sonDeplacement = new MediaPlayer();
                    sonDeplacement.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/sons/v8.mp3"));
                    sonDeplacement.Volume = 0.2; // ajustement du volume 
                    sonDeplacement.MediaEnded += RelanceMusique;// appel à la méthode relance pour pas que le son du v8 s'arrête après plusieurs secondes 
                }

                // si le son n'est pas en train de jouer, lance-le
                if (sonDeplacement.Position == TimeSpan.Zero || sonDeplacement.HasAudio == false)
                {
                    sonDeplacement.Play();
                }
            }
        }   

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // arrete le déplacement 
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
            Constances.neiges.Content = "Neiges " + nbNeiges;       // Affiche le nombre neige qui se reactualise 
            if (FenêtreDeDemarrage.modeChrono)
            {
                Constances.neiges.Content = "Neiges " + nbNeiges + "/" + Constances.NOMBREDENEIGE; 
            }             

        }
     


        private bool Collision(double posX, double posY)
        {
            // calcul des dimensions (largeur et longueur) du chasse neige en fonction des coefficients d'échelle
            double largeurChasseNeige = (Constances.TAILLETUILE * Constances.LARGEURCHASSENEIGE - 64) * coefficientTaillefenêtreX;
            double longueurChasseNeige = (Constances.TAILLETUILE * Constances.LONGUEURCHASSENEIGE - 82) * coefficientTaillefenêtreY;
            // Calcul de la hitbox du chasse neige
            double hitboxX = posX + (23 * coefficientTaillefenêtreX);
            double hitboxY = posY + (25 * coefficientTaillefenêtreY);
            // Convertit la position de la hitbox en indices de la grille de la MAP
            int debutX = Math.Clamp((int)(hitboxX / (Constances.TAILLETUILE * coefficientTaillefenêtreX)), 0, Constances.MAP.GetLength(1) - 1);// indice de la première tuile dans la matrice qui intersecte la hitbox.
            int debutY = Math.Clamp((int)(hitboxY / (Constances.TAILLETUILE * coefficientTaillefenêtreY)), 0, Constances.MAP.GetLength(0) - 1); //indice de la première tuile dans la matrice qui intersecte la hitbox.
            int finX = Math.Clamp((int)((hitboxX + largeurChasseNeige) / (Constances.TAILLETUILE * coefficientTaillefenêtreX)), 0, Constances.MAP.GetLength(1) - 1);//indice de la dernière tuile dans la matrice qui intersecte la hitbox.
            int finY = Math.Clamp((int)((hitboxY + longueurChasseNeige) / (Constances.TAILLETUILE * coefficientTaillefenêtreY)), 0, Constances.MAP.GetLength(0) - 1);//indices de la dernière tuile dans la matrice qui intersecte la hitbox.
            // parcourt toutes les cases de la grille dans la zone couverte par la hitbox
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

                        if (rect != null) // si contacte avec la neige le bruit se lance et en même temps la neige s'enlève
                        {
                            Canvas.Children.Remove(rect);           // la neige est enlevé de la map
                            nbNeiges++; // enleve la neige supprimer donc la neige stocké augmente
                            MediaPlayer bruitNeige = new MediaPlayer();
                            bruitNeige.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/sons/neige.mp3"));
                            bruitNeige.Volume = 0.09;
                            bruitNeige.Play();
                        }

                        Constances.TABLEAU_NEIGE[x, y] = null; //supprime le rectangle de neige de la grille
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
