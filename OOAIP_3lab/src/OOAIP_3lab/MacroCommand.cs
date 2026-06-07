namespace OOAIP_3lab;

public class MacroCommand : ICommand
{
    private readonly ICommand[] _commands;

    public MacroCommand(ICommand[] commands)
    {
        if (commands == null || commands.Length == 0)
        {
            throw new ArgumentException("Пустой или некорректный массив команд");
        }
        _commands = commands.ToArray();
    }

    public void Execute()
    {
        _commands.ToList().ForEach(c => c.Execute());
    }
}