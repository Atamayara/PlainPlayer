using PlainPlayer.Data.Exceptions;
using PlainPlayer.Data.Models;

namespace PlainPlayer.Data;

public class AppDataContext
{
    private readonly List<AudioMetadata> songs = [];
    private readonly List<Playlist> playlists = [];

    public List<AudioMetadata> SearchSongs(string keyword)
    {
        return [.. songs
            .Where(s => s.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        s.Artist.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        s.Album.Contains(keyword, StringComparison.OrdinalIgnoreCase))];
    }

    public List<Playlist> SearchPlaylists(string keyword)
    {
        return [.. playlists
            .Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))];
    }

    public List<Playlist> SearchAlbums(string keyword)
    {
        var albumNames = songs.Where(s => s.Album.Contains(keyword, StringComparison.OrdinalIgnoreCase)).Select(s => s.Album).Distinct();
        List<Playlist> albums = [];
        foreach (var albumName in albumNames)
        {
            albums.Add(
                new Playlist(albumName,
                [.. songs.Where(s => s.Album == albumName).Select(s => s.Id)])
            );
        }
        return albums;
    }
    public List<Playlist> SearchArtists(string keyword)
    {
        var artists = songs.Where(s => s.Artist.Contains(keyword, StringComparison.OrdinalIgnoreCase)).Select(s => s.Artist).Distinct();
        List<Playlist> artistSongs = [];
        foreach (var artist in artists)
        {
            artistSongs.Add(
                new Playlist(artist,
                [.. songs.Where(s => s.Artist == artist).Select(s => s.Id)])
            );
        }
        return artistSongs;
    }

    public List<AudioMetadata> GetSongsFromPlaylist(Playlist playlist)
    {
        return [.. playlist.SongIds.Select(id =>
            songs.Find(song => song.Id == id) ?? throw new PlaylistInvalidException("プレイリストの楽曲IDが不正です")
        )];
    }

    public void AddSongs(List<AudioMetadata> new_songs)
    {
        songs.AddRange(new_songs);
    }

    public void AddPlaylist(List<Playlist> new_playlists)
    {
        playlists.AddRange(new_playlists);
    }
}