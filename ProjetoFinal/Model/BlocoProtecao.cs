using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace ProjetoFinal.Model
{
    public class BlocoProtecao
    {
        public Rectangle Corpo { get; private set; }
        private Canvas _gameSpace;
        public int Vida { get; private set; }

        // Define as cores para cada estado de vida do bloco
        private readonly SolidColorBrush _corVidaCheia = new SolidColorBrush(Colors.Cyan);
        private readonly SolidColorBrush _corMeiaVida = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush _corPoucaVida = new SolidColorBrush(Colors.OrangeRed);
        private readonly SolidColorBrush _corCritica = new SolidColorBrush(Colors.Red);


        public BlocoProtecao(Canvas gameSpace, double x, double y)
        {
            _gameSpace = gameSpace;
            Vida = 10; // Vida inicial do bloco é 10

            Corpo = new Rectangle
            {
                Width = 80,
                Height = 40,
                Fill = _corVidaCheia
            };

            Canvas.SetLeft(Corpo, x);
            Canvas.SetTop(Corpo, y);
            _gameSpace.Children.Add(Corpo);
        }

        public void LevarDano()
        {
            Vida--;
            AtualizarCor();
        }

        private void AtualizarCor()
        {
            /// Lógica de cores para 10 de vida
            if (Vida > 7)
            {
                Corpo.Fill = _corVidaCheia; //  10, 9, 8
            }
            else if (Vida > 4)
            {
                Corpo.Fill = _corMeiaVida; //  7, 6, 5
            }
            else if (Vida > 1)
            {
                Corpo.Fill = _corPoucaVida; // E4, 3, 2
            }
            else
            {
                Corpo.Fill = _corCritica; // : 1
            }
        }

        public void Destruir()
        {
            _gameSpace.Children.Remove(Corpo);
        }
    }
}
