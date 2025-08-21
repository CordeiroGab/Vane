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
        public Canvas? GameSpace { get; set; }
        private DispatcherTimer? _gameTimer;
        private Player? _player;
        private ObservableCollection<Tiro> _tiros;
        private ObservableCollection<Alien> _aliens;
        private ObservableCollection<Tiro> _tirosDosAliens;
        private Random _random = new Random();
        private double _alienVelocidade = 2;
        private int _alienTiroChance = 1;
        private double _alienDescidaVelocidade = 30;
        private int _proximoMarcoDeVida = 1000;

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

                    // Lógica para ganhar vida a cada 1000 pontos
                    if (_player != null && Placar >= _proximoMarcoDeVida)
                    {
                        _player.GanhaVida();
                        _proximoMarcoDeVida += 1000; // Define o próximo marco
                        OnPropertyChanged(nameof(VidasFormatado)); // Atualiza a UI
                    }
                }
            }
        }

        public string PlacarFormatado => $"Placar: {Placar}";
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
            OnPropertyChanged(nameof(VidasFormatado));
            CriarInimigos();

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(16); // Aproximadamente 60 FPS
            _gameTimer.Tick += GameLoop_Tick;
            _gameTimer.Start();
        }

        private void CriarInimigos()
        {
            double startX = 100;
            double startY = 50;
            int espacamento = 80;

            for (int i = 1; i <= 3; i++) // 3 Tipos de aliens (linhas)
            {
                for (int j = 0; j < 10; j++) // 10 aliens por linha
                {
                    _aliens.Add(new Alien(GameSpace, i, startX + j * espacamento, startY + (i - 1) * espacamento));
                }
            }
        }

        private void GameLoop_Tick(object? sender, object? e)
        {
            if (_player == null) return;

            _player.Mover();

            // Lógica de movimento dos tiros do jogador
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

            // Lógica de movimento dos tiros dos aliens
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
            
            // Lógica de movimento dos aliens
            bool inverterDirecao = false;
            foreach (var alien in _aliens)
            {
                alien.Velocidade = _alienVelocidade;
                alien.Mover();

                double left = Canvas.GetLeft(alien.Corpo);
                if (GameSpace != null && (left <= 0 || left >= GameSpace.ActualWidth - alien.Corpo.Width))
                {
                    inverterDirecao = true;
                }

                // Verifica se o alien alcançou a nave (Fim de Jogo)
                double alienBottom = Canvas.GetTop(alien.Corpo) + alien.Corpo.Height;
                double playerTop = Canvas.GetTop(_player.Corpo);
                if (alienBottom >= playerTop)
                {
                    FimDeJogo();
                    return; // Sai do loop imediatamente
                }
            }

            if (inverterDirecao)
            {
                foreach (var alien in _aliens)
                {
                    alien.Direcao *= -1; // Inverte a direção
                    alien.MoverParaBaixo(_alienDescidaVelocidade);
                }
                 _alienVelocidade += 0.5; // Aumenta a velocidade
            }

            // Lógica de tiro dos aliens
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
            if (_player == null) return;

            // Colisão: Tiro do Player com Alien
            for (int i = _tiros.Count - 1; i >= 0; i--)
            {
                var tiro = _tiros[i];
                var tiroRect = new Windows.Foundation.Rect(Canvas.GetLeft(tiro.Corpo), Canvas.GetTop(tiro.Corpo), tiro.Corpo.Width, tiro.Corpo.Height);

                for (int j = _aliens.Count - 1; j >= 0; j--)
                {
                    var alien = _aliens[j];
                    var alienRect = new Windows.Foundation.Rect(Canvas.GetLeft(alien.Corpo), Canvas.GetTop(alien.Corpo), alien.Corpo.Width, alien.Corpo.Height);

                    if (tiroRect.Left < alienRect.Right && tiroRect.Right > alienRect.Left &&
                        tiroRect.Top < alienRect.Bottom && tiroRect.Bottom > alienRect.Top)
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

            // Colisão: Tiro do Alien com Player
            for (int i = _tirosDosAliens.Count - 1; i >= 0; i--)
            {
                var tiro = _tirosDosAliens[i];
                var tiroRect = new Windows.Foundation.Rect(Canvas.GetLeft(tiro.Corpo), Canvas.GetTop(tiro.Corpo), tiro.Corpo.Width, tiro.Corpo.Height);
                var playerRect = new Windows.Foundation.Rect(Canvas.GetLeft(_player.Corpo), Canvas.GetTop(_player.Corpo), _player.Corpo.Width, _player.Corpo.Height);

                if (tiroRect.Left < playerRect.Right && tiroRect.Right > playerRect.Left &&
                    tiroRect.Top < playerRect.Bottom && tiroRect.Bottom > playerRect.Top)
                {
                    tiro.Destruir();
                    _tirosDosAliens.RemoveAt(i);
                    _player.PerdeVida();
                    OnPropertyChanged(nameof(VidasFormatado));

                    if (_player.Vidas <= 0)
                    {
                        FimDeJogo();
                    }
                    break;
                }
            }
        }
        
        private void FimDeJogo()
        {
            _gameTimer?.Stop();
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
            if (_player != null)
            {
                if (direction == "Left") _player.Direcao = -1;
                else if (direction == "Right") _player.Direcao = 1;
            }
        }

        [RelayCommand]
        private void StopMoving(string direction)
        {
            if (_player != null)
            {
                if ((direction == "Left" && _player.Direcao == -1) ||
                    (direction == "Right" && _player.Direcao == 1))
                {
                    _player.Direcao = 0;
                }
            }
        }

        [RelayCommand]
        private void Atirar()
        {
            if (_player != null)
            {
                double playerLeft = Canvas.GetLeft(_player.Corpo);
                double playerTop = Canvas.GetTop(_player.Corpo);
                _tiros.Add(new Tiro(GameSpace, playerLeft, playerTop, true));
            }
        }
    }
}
