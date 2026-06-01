using PlainPlayer.Player.Audio;

namespace PlainPlayer.App.Commands;

public class PrevCommand(AudioPlayer _player) : ICommand
{
    public string Name => "Prev";
    public string Description => "最初から再生します";
    private readonly AudioPlayer player = _player;
    public void Execute()
    {
        player.Prev();
    }
}