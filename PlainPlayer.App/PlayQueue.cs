using PlainPlayer.Data.Models;

namespace PlainPlayer.App;

public class PlayQueue
{
    private List<AudioMetadata> queue = [];
    private int index = -1;
    public List<AudioMetadata> GetQueue()
    {
        var start = index < 0 ? 0 : index;
        return queue[start..];
    }
    public bool TryMoveAndGetSong(int diff, out AudioMetadata audio)
    {
        if (diff >= queue.Count - index || -diff > index)
        {
            audio = new AudioMetadata("", "", "", "", "", 0, 0, 0);
            return false;
        }
        index += diff;
        audio = queue[index];
        return true;
    }
    public void AddSongs(List<AudioMetadata> songs)
    {
        queue.AddRange(songs);
    }

    public void InsertSongs(int index, List<AudioMetadata> songs)
    {
        queue.InsertRange(this.index + index, songs);
    }
}