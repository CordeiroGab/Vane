using CommunityToolkit.Mvvm.ComponentModel;
using ProjetoFinal.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProjetoFinal.ViewModels
{
    public partial class ScoresViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ScoreEntry> _scores;

        public ScoresViewModel()
        {
            Scores = new ObservableCollection<ScoreEntry>();
        }

        public async Task LoadScoresAsync()
        {
            var loadedScores = await ScoreManager.LoadScoresAsync();
            Scores.Clear();
            foreach (var score in loadedScores)
            {
                Scores.Add(score);
            }
        }
    }
}
