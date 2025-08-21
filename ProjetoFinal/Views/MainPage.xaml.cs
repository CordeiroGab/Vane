using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ProjetoFinal.ViewModels;
using Windows.System;

namespace ProjetoFinal
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }
        private bool _isGameInitialized = false;

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Garante que o jogo só seja inicializado uma vez.
            if (!_isGameInitialized)
            {
                _isGameInitialized = true;

                // Prepara o ViewModel e o foco
                ViewModel.GameSpace = GameSpace;
                GameSpace.Focus(FocusState.Programmatic);

                // Inicia o jogo imediatamente após a página ser carregada
                ViewModel.IniciarJogo();
            }
            
            // O manipulador de SizeChanged não é mais necessário para iniciar o jogo.
            // Você pode remover o método GameSpace_SizeChanged e a linha abaixo.
            // GameSpace.SizeChanged -= GameSpace_SizeChanged;
        }

        // O método GameSpace_SizeChanged pode ser completamente removido.
        /*
        private void GameSpace_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Lógica de inicialização removida daqui
        }
        */

        private void Espaco_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    ViewModel.StartMovingCommand.Execute("Left");
                    break;
                case VirtualKey.Right:
                    ViewModel.StartMovingCommand.Execute("Right");
                    break;
                case VirtualKey.Space:
                    if (!e.KeyStatus.WasKeyDown)
                    {
                        ViewModel.AtirarCommand.Execute(null);
                    }
                    break;
            }
        }

        private void Espaco_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    ViewModel.StopMovingCommand.Execute("Left");
                    break;
                case VirtualKey.Right:
                    ViewModel.StopMovingCommand.Execute("Right");
                    break;
            }
        }
    }
}
