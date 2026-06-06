namespace SpaceBattle;

public sealed class CommandInjectableCommand : ICommand, ICommandInjectable
{
    private ICommand? _command;

    public void Inject(ICommand command)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public void Execute()
    {
        if (_command is null)
        {
            throw new InvalidOperationException("Command was not injected.");
        }

        _command.Execute();
    }
}
