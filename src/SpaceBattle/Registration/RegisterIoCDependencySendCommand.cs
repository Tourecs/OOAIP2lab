namespace SpaceBattle;

public sealed class RegisterIoCDependencySendCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Send", args => new SendCommand((ICommand)args[0], (ICommandReceiver)args[1]));
    }
}
