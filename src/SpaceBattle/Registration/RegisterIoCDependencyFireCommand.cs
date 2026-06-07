namespace SpaceBattle;

public sealed class RegisterIoCDependencyFireCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Fire",
            (Func<object[], object>)(args =>
            {
                var ship = (IDictionary<string, object>)args[0];
                return new FireCommand(ship);
            }));
    }
}