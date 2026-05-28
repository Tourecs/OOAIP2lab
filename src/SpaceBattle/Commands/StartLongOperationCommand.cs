namespace SpaceBattle;

public sealed class StartLongOperationCommand : ICommand
{
    private readonly IDictionary<string, object> _order;

    public StartLongOperationCommand(IDictionary<string, object> order)
    {
        _order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public void Execute()
    {
        var operation = (string)_order["operation"];
        var gameObject = _order["object"];
        var receiver = (ICommandReceiver)_order["receiver"];

        var longCommand = Ioc.Resolve<ICommand>($"Macro.{operation}", gameObject);
        var injectableCommand = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");
        injectableCommand.Inject(longCommand);

        _order["command"] = injectableCommand;
        Ioc.Resolve<ICommand>("Commands.Send", injectableCommand, receiver).Execute();
    }
}
