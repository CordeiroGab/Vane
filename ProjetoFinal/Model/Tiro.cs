using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace ProjetoFinal.Model
{
    public class Tiro
    {
        public Image Corpo { get; private set; }
        private readonly Canvas _gameSpace;
        private int _direcao; // -1 para cima (jogador), 1 para baixo (alien)

        public Tiro(Canvas gameSpace, double x, double y, bool tiroDoPlayer)
        {
            _gameSpace = gameSpace;

            Corpo = new Image
            {
                Width = 8,
                Height = 20,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/ArquivoJogo/Tiro.png"))
            };

            if (tiroDoPlayer)
            {
                _direcao = -1; // Move para cima
                Canvas.SetLeft(Corpo, x + 70 - Corpo.Width / 2);
                Canvas.SetTop(Corpo, y - Corpo.Height);
            }
            else
            {
                _direcao = 1; // Move para baixo
                Canvas.SetLeft(Corpo, x);
                Canvas.SetTop(Corpo, y);
            }


            _gameSpace.Children.Add(Corpo);
        }

        public void Mover()
        {
            double top = Canvas.GetTop(Corpo);
            Canvas.SetTop(Corpo, top + (15 * _direcao));
        }

        public bool ForaDaTela()
        {
            double top = Canvas.GetTop(Corpo);
            return top < -20 || top > _gameSpace.ActualHeight;
        }

        public void Destruir()
        {
            _gameSpace.Children.Remove(Corpo);
        }
    }
}
