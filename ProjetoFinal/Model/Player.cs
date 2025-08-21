using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace ProjetoFinal.Model
{
    public class Player
    {
        public Image Corpo { get; private set; }
        private Canvas _gameSpace;
        public int Direcao { get; set; } // -1 para Esquerda, 1 para Direita, 0 para Parado
        private double Velocidade { get; } = 10;
        public int Vidas { get; private set; } = 5;

        public Player(Canvas gameSpace)
        {
            _gameSpace = gameSpace;
            Corpo = new Image
            {
                Width = 140,
                Height = 100,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/ArquivoJogo/Nave.png"))
            };

            Canvas.SetLeft(Corpo, (gameSpace.ActualWidth / 2) - (Corpo.Width / 2));
            Canvas.SetTop(Corpo, gameSpace.ActualHeight - Corpo.Height - 20);
            _gameSpace.Children.Add(Corpo);
        }

        public void Mover()
        {
            double left = Canvas.GetLeft(Corpo);
            double novaPosicao = left + (Direcao * Velocidade);

            if (novaPosicao > 0 && novaPosicao < _gameSpace.ActualWidth - Corpo.Width)
            {
                Canvas.SetLeft(Corpo, novaPosicao);
            }
        }

        public void PerdeVida()
        {
            Vidas--;
        }

        // Adicionado: Método para o jogador ganhar uma vida
        public void GanhaVida()
        {
            Vidas++;
        }
    }
}
