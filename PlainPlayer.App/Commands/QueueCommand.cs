using PlainPlayer.Data.Models;

namespace PlainPlayer.App.Commands;

public class QueueCommand(List<AudioMetadata> _queue, ref int _currentIndex) : ICommand
{
    public string Name => "Queue";
    public string Description => "現在のキューを表示します";
    private readonly List<AudioMetadata> queue = _queue;
    private readonly int currentIndex = _currentIndex;
    public void Execute()
    {
        if (queue.Count == 0)
        {
            Console.WriteLine("キューは空です\n");
            return;
        }
        for (int i = queue.Count - 1; i >= Math.Max(currentIndex, 0); i--)
            Console.WriteLine($"[{i}] {queue[i]}");
        Console.WriteLine();
    }
}