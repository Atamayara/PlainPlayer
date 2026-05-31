namespace PlainPlayer.Data.Models;

public record Playlist(
    string Name,
    List<string> SongIds
)
{
    override public string ToString() => Name;
}