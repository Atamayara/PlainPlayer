using PlainPlayer.App.Commands;
using PlainPlayer.Data;
using PlainPlayer.Data.Models;
using PlainPlayer.Data.Settings;
using PlainPlayer.IO.Loaders;
using PlainPlayer.IO.Settings;
using PlainPlayer.Player.Audio;
using PlainPlayer.Player.Exceptions;
using YamlDotNet.Serialization;

namespace PlainPlayer.App;

class Program
{
    private static readonly AudioPlayer player = new();
    private static readonly List<AudioMetadata> queue = [];
    private static int currentIndex = -1;
    static public void Main(string[] args)
    {
        Console.WriteLine("PlainPlayer - v0.0.1");

        player.StateChanged += OnStateChanged;

        var appData = new AppDataContext();

        var settings = new SettingFileManager().Load("./settings.yml");


        if (!string.IsNullOrEmpty(settings.RekordboxLibraryPath))
        {
            var (rekordboxSongs, rekordboxPlaylists) = new RekordboxXmlLoader().Load(settings.RekordboxLibraryPath);
            appData.AddSongs(rekordboxSongs);
            appData.AddPlaylist(rekordboxPlaylists);
            Console.WriteLine($"Notice: rekordboxのライブラリを読み込みました ({settings.RekordboxLibraryPath})");
        }

        var commands = new Dictionary<ConsoleKey, ICommand>
        {
            { ConsoleKey.Spacebar, new PlayCommand(player) },
            { ConsoleKey.S, new SearchCommand(queue, appData, ref currentIndex)},
            { ConsoleKey.Q, new QueueCommand(queue, ref currentIndex)},
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
        if (currentIndex + diff >= queue.Count || currentIndex + diff < 0) return false;
        else
        {
            try
            {
                currentIndex += diff;
                player.Load(queue[currentIndex].Path);
                player.Play();
            }
            catch (StreamCreateFailedException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            return true;
        }
    }

    private static void OnStateChanged(object? sender, PlayingStateChangedEventArgs e)
    {
        switch (e.Reason)
        {
            case PlayingReason.Finished:
                if (!LoadSong(1)) Console.WriteLine("\n⏸ キューが空になりました\n");
                return;

            case PlayingReason.Error:
                if (!LoadSong(1)) Console.WriteLine($"Error: {e.Message}");
                return;

            case PlayingReason.SystemAction:
                if (e.NewState == PlayingState.RequestPrevious)
                    LoadSong(-1);
                return;
        }
        switch (e.NewState)
        {
            case PlayingState.Playing:
                Console.WriteLine($"\n▶ {queue[currentIndex]}\n");
                break;

            case PlayingState.Paused:
                Console.WriteLine($"\n⏸ {queue[currentIndex]}\n");
                break;
        }
    }
}