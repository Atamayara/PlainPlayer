using PlainPlayer.Data.Models;

namespace PlainPlayer.App.Commands;

public class QueueCommand(PlayQueue _queue) : ICommand
{
    public string Name => "Queue";
    public string Description => "現在のキューを表示します";
    private readonly PlayQueue queue = _queue;
    public void Execute()
    {
        var currentQueue = queue.GetQueue();
        if (currentQueue.Count == 0)
        {
            Console.WriteLine("キューは空です\n");
            return;
        }
        for (int i = currentQueue.Count - 1; i >= 0; i--)
            Console.WriteLine($"[{i}] {currentQueue[i]}");
        Console.WriteLine();
    }
}