using PlainPlayer.Player.Audio;

namespace PlainPlayer.App.Commands;

public class CloseCommand(AudioPlayer _player) : ICommand
{
    public string Name => "Close";
    public string Description => "アプリケーションを終了する";
    private readonly AudioPlayer player = _player;
    public void Execute()
    {
        Console.WriteLine("アプリケーションを終了しています...");
        player.Stop();
        Environment.Exit(0);
    }
}