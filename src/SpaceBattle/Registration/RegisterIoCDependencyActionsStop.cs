namespace SpaceBattle;

public sealed class RegisterIoCDependencyActionsStop : ICommand
{
    public void Execute()
    {
        Ioc.Register("Actions.Stop", args => new StopLongOperationCommand((IDictionary<string, object>)args[0]));
    }
}
