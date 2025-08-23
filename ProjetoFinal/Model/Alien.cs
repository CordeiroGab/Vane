using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace ProjetoFinal.Model
{
    public class Alien
    {
        public Image Corpo { get; private set; }
        public int Pontos { get; private set; } // Propriedade para guardar os pontos
        private Canvas _gameSpace;
        public double Velocidade { get; set; } = 5; // Velocidade de movimento do alien
        public int Direcao { get; set; } = 1; // 1 para direita, -1 para esquerda
        private static Random _random = new Random(); // Adicionado para gerar pontos aleatórios

        public Alien(Canvas gameSpace, int tipo, double x, double y)
        {
            _gameSpace = gameSpace;
            string nomeImagem;

            // Define os pontos baseado no tipo (linha) do alien
            switch (tipo)
            {
                case 1: // Fileira de baixo
                    Pontos = 30;
                    nomeImagem = "Alien1";
                    break;
                case 2: // Fileira do meio
                    Pontos = 20;
                    nomeImagem = "Alien2";
                    break;
                case 3: // Fileira de cima
                    Pontos = 10;
                    nomeImagem = "Alien3";
                    break;
                case 4: // Alien Vermelho
                    Pontos = _random.Next(30, 101); // Pontos aleatórios entre 30 e 100
                    nomeImagem = "AlienVermelho";
                    break;
                default:
                    Pontos = 10;
                    nomeImagem = "Alien1";
                    break;
            }
            Corpo = new Image()
            {
                Width = 60,
                Height = 60,
                Source = new BitmapImage(new Uri($"ms-appx:///Assets/ArquivoJogo/{nomeImagem}.png"))
            };

            Canvas.SetLeft(Corpo, x);
            Canvas.SetTop(Corpo, y);

            //ADICIONA O CORPO DO ALIEN NO CANVAS DO JOGO
            _gameSpace.Children.Add(Corpo);
        }

        // MOVE O ALIEN HORIZONTALMENTE
        public void Mover()
        {
            double left = Canvas.GetLeft(Corpo);
            Canvas.SetLeft(Corpo, left + Velocidade * Direcao);
        }
        
        // ALTERADO: MOVE O ALIEN VERTICALMENTE com uma distância customizável
        public void MoverParaBaixo(double distancia)
        {
            double top = Canvas.GetTop(Corpo);
            Canvas.SetTop(Corpo, top + distancia); // Usa a distância recebida
        }


        // REMOVE O ALIEN DO JOGO
        public void Destruir()
        {
            _gameSpace.Children.Remove(Corpo);
        }
    }
}
