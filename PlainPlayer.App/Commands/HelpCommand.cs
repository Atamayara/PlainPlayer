namespace PlainPlayer.App.Commands;

public class HelpCommand(IReadOnlyDictionary<ConsoleKey, ICommand> _commands) : ICommand
{
    public string Name => "Help";
    public string Description => "ヘルプを表示する";
    private readonly IReadOnlyDictionary<ConsoleKey, ICommand> commands = _commands;
    public void Execute()
    {
        Console.WriteLine();
        foreach (var (key, command) in commands)
        {
            Console.WriteLine($"{key} - {command.Name} / {command.Description}");
        }
        Console.WriteLine();
    }
}