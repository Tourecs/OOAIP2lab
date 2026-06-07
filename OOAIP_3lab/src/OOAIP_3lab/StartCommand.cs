namespace OOAIP_3lab;

public class StartCommand : ICommand
{
    private readonly IDictionary<string, object> _gameObject;
    private readonly string _cmdType;
    private readonly ICommandReceiver _receiver;

    public StartCommand(IDictionary<string, object> gameObject, string cmdType)
    {
        _gameObject = gameObject;
        _cmdType = cmdType;

        if (!_gameObject.TryGetValue("Receiver", out var receiverObj) ||
            receiverObj is not ICommandReceiver receiver)
        {
            throw new InvalidOperationException("В игровом объекте отсутствует ICommandReceiver");
        }
        _receiver = receiver;
    }

    public void Execute()
    {
        var cmd = Ioc.Resolve<ICommand>($"Commands.{_cmdType}", _gameObject);

        var injectable = (ICommandInjectable)Ioc.Resolve<ICommand>("Commands.CommandInjectable");

        var send = new SendCommand((ICommand)injectable, _receiver);

        var macro = Ioc.Resolve<ICommand>($"Macro.{_cmdType}", cmd, send);

        injectable.Inject(macro);

        _gameObject[$"repeatable{_cmdType}"] = injectable;

        var sendCommand = new SendCommand(macro, _receiver);
        sendCommand.Execute();
    }
}