namespace SpaceBattle;

public sealed class RegisterIoCDependencyActionsStart : ICommand
{
    public void Execute()
    {
        Ioc.Register("Actions.Start", args => new StartLongOperationCommand((IDictionary<string, object>)args[0]));
    }
}
