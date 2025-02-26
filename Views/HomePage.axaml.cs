using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Timers; // Utilisation de System.Timers.Timer

namespace KitBox_Project.Views
{
    public partial class HomePage : UserControl
    {
        private double _position = -300; // Position initiale du texte (en dehors de l'écran à gauche)
        private Timer? _timer; // Déclare le timer comme nullable

        public HomePage()
        {
            InitializeComponent();
            StartAnimation();
        }

        private void StartAnimation()
        {
            var movingText = this.FindControl<TextBlock>("MovingText");

            if (movingText != null) // Vérifie si movingText n'est pas null
            {
                // Crée un Timer pour mettre à jour la position
                _timer = new Timer(20); // Intervalle de mise à jour pour l'animation (en millisecondes)
                _timer.Elapsed += (sender, e) =>
                {
                    // Met à jour la position du texte dans un thread sécurisé
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        // Déplace le texte de gauche à droite
                        _position += 3; // La vitesse de déplacement
                        if (_position > this.Bounds.Width)
                        {
                            _position = -movingText.Bounds.Width; // Recommence quand le texte est entièrement à droite
                        }
                        movingText.RenderTransform = new Avalonia.Media.TranslateTransform(_position, 30); // Applique la translation
                    });
                };
                _timer.Start(); // Démarre l'animation
            }
        }
    }
}
