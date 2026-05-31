using PlainPlayer.Data.Models;

namespace PlainPlayer.Data.Interfaces;

public interface ILibraryLoader
{
    (List<AudioMetadata> Songs, List<Playlist> Playlists) Load(string path);
}