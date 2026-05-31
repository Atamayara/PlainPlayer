using PlainPlayer.Player.Audio;

namespace PlainPlayer.App.Commands;

public class PlayCommand(AudioPlayer _player) : ICommand
{
    public string Name => "Play/Pause";
    public string Description => "再生する/一時停止する";
    private readonly AudioPlayer player = _player;
    public void Execute()
    {
        player.PlayOrPause();
    }
}