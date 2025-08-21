using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ProjetoFinal.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjetoFinal.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public Canvas GameSpace { get; set; }
        private DispatcherTimer _gameTimer;
        private Player _player;
        private ObservableCollection<Tiro> _tiros;
        private ObservableCollection<Alien> _aliens;
        private ObservableCollection<Tiro> _tirosDosAliens;
        private Random _random = new Random();
        private double _alienVelocidade = 2;
        private int _alienTiroChance = 1;
        private double _alienDescidaVelocidade = 30;

        private int _placar;
        public int Placar
        {
            get => _placar;
            set
            {
                if (SetProperty(ref _placar, value))
                {
                    OnPropertyChanged(nameof(Placar));
                    OnPropertyChanged(nameof(PlacarFormatado));
                }
            }
        }

        public string PlacarFormatado => $"Placar: {Placar}";

        // Adicionado: Propriedade para formatar e exibir as vidas
        public string VidasFormatado => $"Vidas: {_player?.Vidas ?? 0}";

        public MainViewModel()
        {
            _tiros = new ObservableCollection<Tiro>();
            _aliens = new ObservableCollection<Alien>();
            _tirosDosAliens = new ObservableCollection<Tiro>();
            Placar = 0;
        }

        public void IniciarJogo()
        {
            _player = new Player(GameSpace);
            OnPropertyChanged(nameof(VidasFormatado)); // Adicionado: Notifica a UI para atualizar as vidas no início
            CriarInimigos();

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            _gameTimer.Tick += GameLoop_Tick;
            _gameTimer.Start();
        }

        private void CriarInimigos()
        {
            double startX = 100;
            double startY = 50;
            int espacamento = 80;

            for (int i = 1; i <= 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _aliens.Add(new Alien(GameSpace, i, startX + j * espacamento, startY + (i - 1) * espacamento));
                }
            }
        }

        private void GameLoop_Tick(object sender, object e)
        {
            _player.Mover();

            for (int i = _tiros.Count - 1; i >= 0; i--)
            {
                var tiro = _tiros[i];
                tiro.Mover();
                if (tiro.ForaDaTela())
                {
                    tiro.Destruir();
                    _tiros.RemoveAt(i);
                }
            }

            for (int i = _tirosDosAliens.Count - 1; i >= 0; i--)
            {
                var tiro = _tirosDosAliens[i];
                tiro.Mover();
                if (tiro.ForaDaTela())
                {
                    tiro.Destruir();
                    _tirosDosAliens.RemoveAt(i);
                }
            }

            bool inverterDirecao = false;
            foreach (var alien in _aliens)
            {
                alien.Velocidade = _alienVelocidade;
                alien.Mover();

                double left = Canvas.GetLeft(alien.Corpo);
                if (left <= 0 || left >= GameSpace.ActualWidth - alien.Corpo.Width)
                {
                    inverterDirecao = true;
                }
            }

            if (inverterDirecao)
            {
                foreach (var alien in _aliens)
                {
                    alien.Direcao *= -1;
                    alien.MoverParaBaixo(_alienDescidaVelocidade);
                }
                _alienVelocidade += 0.5;
            }

            if (_random.Next(100) < _alienTiroChance && _aliens.Any())
            {
                var atirador = _aliens[_random.Next(_aliens.Count)];
                double x = Canvas.GetLeft(atirador.Corpo) + atirador.Corpo.Width / 2;
                double y = Canvas.GetTop(atirador.Corpo) + atirador.Corpo.Height;
                _tirosDosAliens.Add(new Tiro(GameSpace, x, y, false));
            }

            DetectarColisoes();
        }

        private void DetectarColisoes()
        {
            for (int i = _tiros.Count - 1; i >= 0; i--)
            {
                var tiro = _tiros[i];
                var tiroRect = new Windows.Foundation.Rect(Canvas.GetLeft(tiro.Corpo), Canvas.GetTop(tiro.Corpo), tiro.Corpo.Width, tiro.Corpo.Height);

                for (int j = _aliens.Count - 1; j >= 0; j--)
                {
                    var alien = _aliens[j];
                    var alienRect = new Windows.Foundation.Rect(Canvas.GetLeft(alien.Corpo), Canvas.GetTop(alien.Corpo), alien.Corpo.Width, alien.Corpo.Height);

                    if (tiroRect.Left < alienRect.Right &&
                        tiroRect.Right > alienRect.Left &&
                        tiroRect.Top < alienRect.Bottom &&
                        tiroRect.Bottom > alienRect.Top)
                    {
                        tiro.Destruir();
                        _tiros.RemoveAt(i);
                        Placar += alien.Pontos;
                        alien.Destruir();
                        _aliens.RemoveAt(j);
                        if (!_aliens.Any())
                        {
                            ProximoNivel();
                        }
                        break;
                    }
                }
            }

            for (int i = _tirosDosAliens.Count - 1; i >= 0; i--)
            {
                var tiro = _tirosDosAliens[i];
                var tiroRect = new Windows.Foundation.Rect(Canvas.GetLeft(tiro.Corpo), Canvas.GetTop(tiro.Corpo), tiro.Corpo.Width, tiro.Corpo.Height);
                var playerRect = new Windows.Foundation.Rect(Canvas.GetLeft(_player.Corpo), Canvas.GetTop(_player.Corpo), _player.Corpo.Width, _player.Corpo.Height);

                if (tiroRect.Left < playerRect.Right &&
                    tiroRect.Right > playerRect.Left &&
                    tiroRect.Top < playerRect.Bottom &&
                    tiroRect.Bottom > playerRect.Top)
                {
                    tiro.Destruir();
                    _tirosDosAliens.RemoveAt(i);
                    _player.PerdeVida();
                    OnPropertyChanged(nameof(VidasFormatado)); // Adicionado: Notifica a UI de que as vidas mudaram

                    if (_player.Vidas <= 0)
                    {
                        _gameTimer.Stop();
                    }
                    break;
                }
            }
        }

        private void ProximoNivel()
        {
            _alienTiroChance += 1;
            _alienDescidaVelocidade += 5;
            CriarInimigos();
        }

        [RelayCommand]
        private void StartMoving(string direction)
        {
            if (direction == "Left")
            {
                _player.Direcao = -1;
            }
            else if (direction == "Right")
            {
                _player.Direcao = 1;
            }
        }

        [RelayCommand]
        private void StopMoving(string direction)
        {
            if ((direction == "Left" && _player.Direcao == -1) ||
                (direction == "Right" && _player.Direcao == 1))
            {
                _player.Direcao = 0;
            }
        }

        [RelayCommand]
        private void Atirar()
        {
            double playerLeft = Canvas.GetLeft(_player.Corpo);
            double playerTop = Canvas.GetTop(_player.Corpo);
            _tiros.Add(new Tiro(GameSpace, playerLeft, playerTop, true));
        }
    }
}
