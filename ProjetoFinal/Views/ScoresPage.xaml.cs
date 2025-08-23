using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ProjetoFinal.ViewModels;

namespace ProjetoFinal.Views
{
    public sealed partial class ScoresPage : Page
    {
        public ScoresViewModel ViewModel { get; }

        public ScoresPage()
        {
            this.InitializeComponent();
            ViewModel = new ScoresViewModel();
            this.Loaded += ScoresPage_Loaded;
        }

        private async void ScoresPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadScoresAsync();
            ScoresListView.ItemsSource = ViewModel.Scores;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
