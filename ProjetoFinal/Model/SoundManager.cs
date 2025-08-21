using Windows.Media.Core;
using Windows.Media.Playback;

namespace ProjetoFinal.Model;

public static class SoundManager
{
    private static MediaPlayer _tiroPlayer;
    private static MediaPlayer _explosaoPlayer;

    static SoundManager()
    {
        _tiroPlayer = new MediaPlayer();
        _explosaoPlayer = new MediaPlayer();
    }

    public static void PLayTiroSound()
    {
        _tiroPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Sons/SomTiro.mp3"));
        _tiroPlayer.Play();
    }

    public static void PLayExplosaoSound()
    {
        _explosaoPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Sons/SomExplosão.mp4"));
        _explosaoPlayer.Play();
    }
}
