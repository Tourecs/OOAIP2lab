namespace OOAIP_3lab;

public class CommandInjectableCommand : ICommand, ICommandInjectable
{
    private ICommand? _command;

    public void Execute()
    {
        if (_command == null)
        {
            throw new InvalidOperationException("Command was not injected.");
        }
        _command.Execute();
    }

    public void Inject(ICommand command)
    {
        _command = command;
    }
}