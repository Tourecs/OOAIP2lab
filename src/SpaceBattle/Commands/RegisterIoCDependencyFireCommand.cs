namespace SpaceBattle;

public class RegisterIoCDependencyFireCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Fire", args =>
        {
            var ship = (IDictionary<string, object>)args[0];
            return new FireCommand(ship);
        });
    }
}
