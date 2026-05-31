using System.Xml.Linq;
using PlainPlayer.Data.Interfaces;
using PlainPlayer.Data.Models;

namespace PlainPlayer.IO.Loaders;

public class RekordboxXmlLoader : ILibraryLoader
{
    public (List<AudioMetadata> Songs, List<Playlist> Playlists) Load(string path)
    {
        string[] acceptedFormats = ["MP3 ファイル", "WAV ファイル", "AIFF ファイル", "M4A ファイル"];
        var document = XDocument.Load(path);

        var songs = new List<AudioMetadata>();
        var playlists = new List<Playlist>();

        foreach (var e in document.Root?.Elements("COLLECTION").Elements("TRACK").Where(e => acceptedFormats.Contains(e.Attribute("Kind")?.Value)) ?? [])
        {
            var id = e.Attribute("TrackID")?.Value ?? "";
            var filepath = Uri.UnescapeDataString(e.Attribute("Location")?.Value ?? "").Replace("file://localhost", ""); // [TODO]: Windows?
            var title = e.Attribute("Name")?.Value ?? "不明なタイトル";
            var artist = e.Attribute("Artist")?.Value ?? "不明なアーティスト";
            var album = e.Attribute("Album")?.Value ?? "不明なアルバム";
            if (int.TryParse(e.Attribute("TrackNumber")?.Value, out int trackNumber))
                trackNumber = 0;
            if (int.TryParse(e.Attribute("DiscNumber")?.Value, out int discNumber))
                discNumber = 0;
            if (int.TryParse(e.Attribute("TotalTime")?.Value, out int duration))
                duration = 0;

            songs.Add(new AudioMetadata(
                id, filepath, title, artist, album,  trackNumber, discNumber, duration
            ));
        }


        foreach (var e in document.Root?.Elements("PLAYLISTS").Elements("NODE").Elements("NODE") ?? [])
        {
            var songIds = e.Elements("TRACK").Select(q => q.Attribute("Key")?.Value ?? "").ToList();
            playlists.Add(new Playlist(
                e.Attribute("Name")?.Value ?? "", songIds
            ));
        }

        return (songs, playlists);
    }
}