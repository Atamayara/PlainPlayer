using PlainPlayer.App.Commands;
using PlainPlayer.Data;
using PlainPlayer.Data.Models;
using PlainPlayer.IO.Loaders;
using PlainPlayer.IO.Settings;
using PlainPlayer.Player.Audio;
using PlainPlayer.Player.Exceptions;

namespace PlainPlayer.App;

class Program
{
    private static readonly AudioPlayer player = new();
    private static readonly PlayQueue queue = new();
    static public void Main(string[] args)
    {
        Console.WriteLine("PlainPlayer - v0.1.1q\n");

        player.StateChanged += OnStateChanged;

        var appData = new AppDataContext();

        var settings = new SettingFileManager().Load("settings.yml");


        if (!string.IsNullOrEmpty(settings.RekordboxLibraryPath))
        {
            var (rekordboxSongs, rekordboxPlaylists) = new RekordboxXmlLoader().Load(settings.RekordboxLibraryPath);
            appData.AddSongs(rekordboxSongs);
            appData.AddPlaylist(rekordboxPlaylists);
        }

        Console.WriteLine();

        var commands = new Dictionary<ConsoleKey, ICommand>
        {
            { ConsoleKey.Spacebar, new PlayCommand(player) },
            { ConsoleKey.S, new SearchCommand(queue, appData)},
            { ConsoleKey.Q, new QueueCommand(queue)},
            { ConsoleKey.RightArrow, new SkipCommand(player)},
            { ConsoleKey.LeftArrow, new PrevCommand(player)},
            { ConsoleKey.Escape, new CloseCommand(player)}
        };

        commands.Add(ConsoleKey.H, new HelpCommand(commands));

        while (true)
        {
            Console.Write("コマンドを入力してください\n> ");
            var key = Console.ReadKey(true).Key;
            Console.WriteLine($"{key}\n");
            if (commands.TryGetValue(key, out var command))
                command.Execute();
            else Console.WriteLine("コマンドがありません\n");
        }
    }

    private static bool LoadSong(int diff)
    {
        try
        {
            if (!queue.TryMoveAndGetSong(diff, out AudioMetadata audio))
            {
                return false;
            }
            player.Load(audio.Path);
            player.Play();
        }
        catch (StreamCreateFailedException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        return true;
    }

    private static void OnStateChanged(object? sender, PlayingStateChangedEventArgs e)
    {
        switch (e.Reason)
        {
            case PlayingReason.Finished:
                if (!LoadSong(1)) Console.WriteLine("⏸ キューが空になりました\n");
                break;

            case PlayingReason.Error:
                if (!LoadSong(1)) Console.WriteLine($"Error: {e.Message}\n");
                break;

            default:
                switch (e.NewState)
                {
                    case PlayingState.Playing:
                        Console.WriteLine($"▶ {queue.GetQueue()[0]}\n");
                        break;

                    case PlayingState.Paused:
                        Console.WriteLine($"⏸ {queue.GetQueue()[0]}\n");
                        break;

                    case PlayingState.RequestPrevious:
                        LoadSong(-1);
                        return;
                }
                Console.Write("> ");
                break;
        }
    }
}