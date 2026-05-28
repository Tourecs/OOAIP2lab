namespace SpaceBattle;

public sealed class StopLongOperationCommand : ICommand
{
    private readonly IDictionary<string, object> _order;

    public StopLongOperationCommand(IDictionary<string, object> order)
    {
        _order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public void Execute()
    {
        var injectableCommand = (ICommandInjectable)_order["command"];
        injectableCommand.Inject(new EmptyCommand());
    }
}
