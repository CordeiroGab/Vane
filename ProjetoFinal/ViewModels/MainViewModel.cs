using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ProjetoFinal.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks; // Adicionado

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
        private ObservableCollection<BlocoProtecao> _blocosProtecao;
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

                    if (_player != null && Placar >= _proximoMarcoDeVida)
                    {
                        _player.GanhaVida();
                        _proximoMarcoDeVida += 1000;
                        OnPropertyChanged(nameof(VidasFormatado));
                    }
                }
            }
        }

        public string PlacarFormatado => $"Placar: {Placar}";
        public string VidasFormatado => $"Vidas: {_player?.Vidas ?? 0}";
        
        // Evento para notificar a View sobre o fim do jogo
        public event Func<Task> GameEnded;


        public MainViewModel()
        {
            _tiros = new ObservableCollection<Tiro>();
            _aliens = new ObservableCollection<Alien>();
            _tirosDosAliens = new ObservableCollection<Tiro>();
            _blocosProtecao = new ObservableCollection<BlocoProtecao>();
            Placar = 0;
        }

        public void IniciarJogo()
        {
            _player = new Player(GameSpace);
            OnPropertyChanged(nameof(VidasFormatado));
            CriarInimigos();
            CriarBlocosProtecao();

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

        private void CriarBlocosProtecao()
        {
            foreach (var bloco in _blocosProtecao)
            {
                bloco.Destruir();
            }
            _blocosProtecao.Clear();

            if (_player == null || GameSpace == null) return;

            double alturaNave = Canvas.GetTop(_player.Corpo);
            double larguraTela = GameSpace.ActualWidth;

            double[] posicoes = { larguraTela * 0.2, larguraTela * 0.5, larguraTela * 0.8 };

            foreach (var posX in posicoes)
            {
                _blocosProtecao.Add(new BlocoProtecao(GameSpace, posX - 40, alturaNave - 60));
            }
        }

        private void GameLoop_Tick(object? sender, object? e)
        {
            if (_player == null) return;
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
                if (GameSpace != null && (left <= 0 || left >= GameSpace.ActualWidth - alien.Corpo.Width))
                {
                    inverterDirecao = true;
                }

                double alienBottom = Canvas.GetTop(alien.Corpo) + alien.Corpo.Height;
                double playerTop = Canvas.GetTop(_player.Corpo);
                if (alienBottom >= playerTop)
                {
                    FimDeJogo();
                    return;
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
            if (_player == null) return;

            // Colisão: Tiro do Player
            for (int i = _tiros.Count - 1; i >= 0; i--)
            {
                var tiro = _tiros[i];
                var tiroRect = new Windows.Foundation.Rect(Canvas.GetLeft(tiro.Corpo), Canvas.GetTop(tiro.Corpo), tiro.Corpo.Width, tiro.Corpo.Height);
                bool tiroAtingiuAlgo = false;

                // Com Aliens
                for (int j = _aliens.Count - 1; j >= 0; j--)
                {
                    var alien = _aliens[j];
                    var alienRect = new Windows.Foundation.Rect(Canvas.GetLeft(alien.Corpo), Canvas.GetTop(alien.Corpo), alien.Corpo.Width, alien.Corpo.Height);
                    if (tiroRect.Intersects(alienRect))
                    {
                        tiro.Destruir(); _tiros.RemoveAt(i);
                        Placar += alien.Pontos;
                        alien.Destruir(); _aliens.RemoveAt(j);
                        if (!_aliens.Any()) ProximoNivel();
                        tiroAtingiuAlgo = true;
                        break;
                    }
                }
                if (tiroAtingiuAlgo) continue;

                // Com Blocos
                for (int j = _blocosProtecao.Count - 1; j >= 0; j--)
                {
                    var bloco = _blocosProtecao[j];
                    var blocoRect = new Windows.Foundation.Rect(Canvas.GetLeft(bloco.Corpo), Canvas.GetTop(bloco.Corpo), bloco.Corpo.Width, bloco.Corpo.Height);
                    if (tiroRect.Intersects(blocoRect))
                    {
                        tiro.Destruir(); _tiros.RemoveAt(i);
                        bloco.LevarDano();
                        if (bloco.Vida <= 0) { bloco.Destruir(); _blocosProtecao.RemoveAt(j); }
                        break;
                    }
                }
            }

            // Colisão: Tiro do Alien
            for (int i = _tirosDosAliens.Count - 1; i >= 0; i--)
            {
                var tiro = _tirosDosAliens[i];
                var tiroRect = new Windows.Foundation.Rect(Canvas.GetLeft(tiro.Corpo), Canvas.GetTop(tiro.Corpo), tiro.Corpo.Width, tiro.Corpo.Height);
                bool tiroAtingiuAlgo = false;

                // Com Blocos
                for (int j = _blocosProtecao.Count - 1; j >= 0; j--)
                {
                    var bloco = _blocosProtecao[j];
                    var blocoRect = new Windows.Foundation.Rect(Canvas.GetLeft(bloco.Corpo), Canvas.GetTop(bloco.Corpo), bloco.Corpo.Width, bloco.Corpo.Height);
                    if (tiroRect.Intersects(blocoRect))
                    {
                        tiro.Destruir(); _tirosDosAliens.RemoveAt(i);
                        bloco.LevarDano();
                        if (bloco.Vida <= 0) { bloco.Destruir(); _blocosProtecao.RemoveAt(j); }
                        tiroAtingiuAlgo = true;
                        break;
                    }
                }
                if (tiroAtingiuAlgo) continue;
                
                // Com Player
                var playerRect = new Windows.Foundation.Rect(Canvas.GetLeft(_player.Corpo), Canvas.GetTop(_player.Corpo), _player.Corpo.Width, _player.Corpo.Height);
                if (tiroRect.Intersects(playerRect))
                {
                    tiro.Destruir(); _tirosDosAliens.RemoveAt(i);
                    _player.PerdeVida();
                    OnPropertyChanged(nameof(VidasFormatado));
                    if (_player.Vidas <= 0) FimDeJogo();
                    break;
                }
            }
        }
        
        private async void FimDeJogo()
        {
            _gameTimer?.Stop();
            if (GameEnded != null)
            {
                await GameEnded.Invoke();
            }
        }

        private void ProximoNivel()
        {
            _alienTiroChance += 1;
            _alienDescidaVelocidade += 5;
            CriarInimigos();
            CriarBlocosProtecao();
        }

        [RelayCommand]
        private void StartMoving(string direction)
        {
            if (_player == null) return;
            if (direction == "Left") _player.Direcao = -1;
            else if (direction == "Right") _player.Direcao = 1;
        }

        [RelayCommand]
        private void StopMoving(string direction)
        {
            if (_player == null) return;
            if ((direction == "Left" && _player.Direcao == -1) ||
                (direction == "Right" && _player.Direcao == 1))
            {
                _player.Direcao = 0;
            }
        }

        [RelayCommand]
        private void Atirar()
        {
            if (_player == null) return;
            double playerLeft = Canvas.GetLeft(_player.Corpo);
            double playerTop = Canvas.GetTop(_player.Corpo);
            _tiros.Add(new Tiro(GameSpace, playerLeft, playerTop, true));
        }
    }

    // Classe auxiliar para facilitar a leitura da detecção de colisão
    public static class RectExtensions
    {
        public static bool Intersects(this Windows.Foundation.Rect rectA, Windows.Foundation.Rect rectB)
        {
            return rectA.Left < rectB.Right &&
                   rectA.Right > rectB.Left &&
                   rectA.Top < rectB.Bottom &&
                   rectA.Bottom > rectB.Top;
        }
    }
}
