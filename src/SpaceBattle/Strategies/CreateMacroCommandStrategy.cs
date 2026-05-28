namespace SpaceBattle;

public sealed class CreateMacroCommandStrategy
{
    private readonly string _commandSpec;

    public CreateMacroCommandStrategy(string commandSpec)
    {
        _commandSpec = string.IsNullOrWhiteSpace(commandSpec)
            ? throw new ArgumentException("Command spec is required.", nameof(commandSpec))
            : commandSpec;
    }

    public ICommand Resolve(object[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var commandNames = Ioc.Resolve<IEnumerable<string>>($"Specs.{_commandSpec}");
        var commands = commandNames.Select(commandName => Ioc.Resolve<ICommand>(commandName, args)).ToArray();
        return Ioc.Resolve<ICommand>("Commands.Macro", commands);
    }
}
