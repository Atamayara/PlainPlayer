using PlainPlayer.Player.Audio;

namespace PlainPlayer.App.Commands;

public class SkipCommand(AudioPlayer _player) : ICommand
{
    public string Name => "Skip";
    public string Description => "現在再生中の曲をスキップします";
    private readonly AudioPlayer player = _player;
    public void Execute()
    {
        player.Skip();
    }
}