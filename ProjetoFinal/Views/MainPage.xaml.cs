using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ProjetoFinal.Model;
using ProjetoFinal.ViewModels;
using System.Threading.Tasks;

namespace ProjetoFinal.Views
{
    public sealed partial class MainPage : Page
    {
        // Renomeado para seguir a convenção de nomenclatura
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            ViewModel = new MainViewModel();
            this.InitializeComponent(); // Este método é crucial e deve ser chamado aqui

            // O Loaded event é uma forma mais segura de garantir que o GameSpace foi renderizado
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Passa a referência do Canvas para o ViewModel
            ViewModel.GameSpace = this.GameSpace;
            ViewModel.IniciarJogo();

            // Foca o Canvas para que ele possa receber eventos de teclado
            GameSpace.Focus(FocusState.Programmatic);

            // Assina o evento de fim de jogo
            ViewModel.GameEnded += OnGameEnded;
        }

        private async Task OnGameEnded()
        {
            var nicknameDialog = new ContentDialog
            {
                Title = "Game Over",
                Content = new TextBox { PlaceholderText = "Digite seu apelido" },
                PrimaryButtonText = "Salvar Pontuação",
                CloseButtonText = "Voltar ao Menu",
                XamlRoot = this.XamlRoot
            };

            var result = await nicknameDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var nickname = (nicknameDialog.Content as TextBox)?.Text;
                if (!string.IsNullOrWhiteSpace(nickname))
                {
                    var scoreEntry = new ScoreEntry { Nickname = nickname, Score = ViewModel.Placar };
                    await ScoreManager.SaveScoreAsync(scoreEntry);
                }
            }
    
            // Volta para a página inicial
            this.Frame.Navigate(typeof(StartPage));
        }

        private void Espaco_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Left:
                    ViewModel.StartMovingCommand.Execute("Left");
                    break;
                case Windows.System.VirtualKey.Right:
                    ViewModel.StartMovingCommand.Execute("Right");
                    break;
                case Windows.System.VirtualKey.Space:
                    ViewModel.AtirarCommand.Execute(null);
                    break;
            }
        }

        private void Espaco_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Left:
                    ViewModel.StopMovingCommand.Execute("Left");
                    break;
                case Windows.System.VirtualKey.Right:
                    ViewModel.StopMovingCommand.Execute("Right");
                    break;
            }
        }
    }
}
