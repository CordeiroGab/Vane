using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ProjetoFinal.Views
{
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Navega para a página principal do jogo
            this.Frame.Navigate(typeof(MainPage));
        }

        private void ScoresButton_Click(object sender, RoutedEventArgs e)
        {
            // Navega para a página de scores
            this.Frame.Navigate(typeof(ScoresPage));
        }
    }
}
