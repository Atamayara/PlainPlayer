namespace PlainPlayer.Data.Models;

public record AudioMetadata(
    string Id,
    string Path,
    string Title,
    string Artist,
    string Album,
    int TrackNumber,
    int DiscNumber,
    int Duration
)
{
    override public string ToString() => $"{Title} / {Artist} - {Album} [{Duration / 60}:{Duration % 60}]";
}