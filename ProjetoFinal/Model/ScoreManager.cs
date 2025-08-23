using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace ProjetoFinal.Model
{
    // classe será responsavel por salvar a carregar as pontuações de um arquivo de texto
    public static class ScoreManager
    {
        private const string FileName = "scores.txt";

        public static async Task SaveScoreAsync(ScoreEntry scoreEntry)
        {
            var scoreLine = $"{scoreEntry.Nickname},{scoreEntry.Score}";
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            await FileIO.AppendLinesAsync(file, new[] { scoreLine });
        }

        public static async Task<List<ScoreEntry>> LoadScoresAsync()
        {
            var scores = new List<ScoreEntry>();
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.GetFileAsync(FileName);
                var lines = await FileIO.ReadLinesAsync(file);

                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                    {
                        scores.Add(new ScoreEntry { Nickname = parts[0], Score = score });
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // O arquivo ainda não existe, retorna lista vazia
            }
            return scores.OrderByDescending(s => s.Score).ToList();
        }
    }
}
