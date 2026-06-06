namespace SpaceBattle;

public sealed class MacroCommand : ICommand
{
    private readonly ICommand[] _commands;

    public MacroCommand(params ICommand[] commands)
    {
        ArgumentNullException.ThrowIfNull(commands);
        _commands = commands.ToArray();
    }

    public void Execute() => Array.ForEach(_commands, command => command.Execute());
}
