using PlainPlayer.Data;
using PlainPlayer.Data.Models;

namespace PlainPlayer.App.Commands;

public class SearchCommand(PlayQueue _queue, AppDataContext _appData) : ICommand
{
    public string Name => "Search";
    public string Description => "楽曲 / アルバム / アーティスト / プレイリストを検索します";
    private readonly PlayQueue queue = _queue;
    private readonly AppDataContext appData = _appData;
    public void Execute()
    {
        Console.Write("キーワードを入力してください\n> ");
        var input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("キャンセルされました\n");
            return;
        }

        Console.WriteLine();
        var songs = appData.SearchSongs(input);
        List<Playlist>? playlists = null;
        List<Playlist>? albums = null;
        List<Playlist>? artists = null;
        var page = 0;
        var mode = SearchMode.Songs;
        int index = -1;

        while (index < 0)
        {
            int quantity = 0;
            switch (mode)
            {
                case SearchMode.Songs:
                    quantity = songs.Count;
                    Console.WriteLine($"楽曲 {page + 1}ページ目");
                    if (quantity == 0) break;
                    for (int i = 0; i < 10 && i + page * 10 < songs.Count; i++)
                        Console.WriteLine($"[{(i + 1) % 10}] {songs[i + page * 10]}");
                    break;

                case SearchMode.Playlists:
                    Console.WriteLine($"プレイリスト {page + 1}ページ目");
                    playlists ??= appData.SearchPlaylists(input);
                    quantity = playlists.Count;
                    if (quantity == 0) break;
                    for (int i = 0; i < 10 && i + page * 10 < playlists.Count; i++)
                        Console.WriteLine($"[{(i + 1) % 10}] {playlists[i]}");
                    break;

                case SearchMode.Albums:
                    Console.WriteLine($"アルバム {page + 1}ページ目");
                    albums ??= appData.SearchAlbums(input);
                    quantity = albums.Count;
                    if (quantity == 0) break;
                    for (int i = 0; i < 10 && i + page * 10 < albums.Count; i++)
                        Console.WriteLine($"[{(i + 1) % 10}] {albums[i]}");
                    break;

                case SearchMode.Artists:
                    Console.WriteLine($"アーティスト {page + 1}ページ目");
                    artists ??= appData.SearchArtists(input);
                    quantity = artists.Count;
                    if (quantity == 0) break;
                    for (int i = 0; i < 10 && i + page * 10 < artists.Count; i++)
                        Console.WriteLine($"[{(i + 1) % 10}] {artists[i]}");
                    break;
            }
            if (quantity == 0) Console.WriteLine("検索結果はありません\n");
            Console.WriteLine("[Num]: キューに追加 / Arrow: ページ移動 / P: プレイリスト / A: アルバム / R: アーティスト / S: 楽曲 / C: キャンセル");

            do
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case >= ConsoleKey.D0 and <= ConsoleKey.D9:
                        index = page * 10 + (key - ConsoleKey.D0 - 1 + 10) % 10;
                        if (index >= quantity)
                        {
                            index = -1;
                            continue;
                        }
                        break;

                    case ConsoleKey.UpArrow or ConsoleKey.LeftArrow:
                        page = page < 1 ? 0 : page - 1;
                        break;

                    case ConsoleKey.DownArrow or ConsoleKey.RightArrow:
                        page = (page + 1) * 10 > quantity ? page : page + 1;
                        break;

                    case ConsoleKey.P:
                        mode = SearchMode.Playlists;
                        page = 0;
                        break;
                    case ConsoleKey.A:
                        mode = SearchMode.Albums;
                        page = 0;
                        break;
                    case ConsoleKey.R:
                        mode = SearchMode.Artists;
                        page = 0;
                        break;
                    case ConsoleKey.S:
                        mode = SearchMode.Songs;
                        page = 0;
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine($"{key}");
                        Console.WriteLine("キャンセルされました\n");
                        return;

                    default:
                        continue;
                }
                Console.WriteLine($"{key}\n");
                break;
            } while (true);
        }

        bool isNext = true;
        bool isRandom = false;
        do
        {
            Console.Write($"N: 次に再生 / L: キューの終わりに追加 / R: ランダム再生{(!isRandom ? "する" : "しない")} / C: キャンセル\n> ");
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.N:
                        isNext = true;
                        break;
                    case ConsoleKey.L:
                        isNext = false;
                        break;
                    case ConsoleKey.R:
                        isRandom = !isRandom;
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine("キャンセルされました\n");
                        return;
                    default:
                        continue;
                }
                break;
            } while (true);
            Console.WriteLine($"{key}\n");
            if (key == ConsoleKey.R) continue;
            break;
        } while (true);

        List<AudioMetadata> songList = [];
        switch (mode)
        {
            case SearchMode.Songs:
                if (isNext)
                    songList.Add(songs[index]);
                break;

            case SearchMode.Playlists:
                if (playlists is not null)
                    songList = appData.GetSongsFromPlaylist(playlists[index]);
                break;

            case SearchMode.Albums:
                if (albums is not null)
                    songList = appData.GetSongsFromPlaylist(albums[index]);
                break;

            case SearchMode.Artists:
                if (artists is not null)
                    songList = appData.GetSongsFromPlaylist(artists[index]);
                break;
        }
        if (isRandom)
        {
            var songArray = songList.ToArray();
            Random.Shared.Shuffle(songArray);
            songList = [.. songArray];
        }
        if (isNext)
            queue.InsertSongs(1, songList);
        else
            queue.AddSongs(songList);
    }
}

internal enum SearchMode
{
    Songs,
    Playlists,
    Albums,
    Artists
}