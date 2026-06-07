namespace OOAIP_3lab;

public class RegisterIoCDependencyFireCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Commands.Fire",
            (Func<object[], object>)(args =>
            {
                var ship = (IDictionary<string, object>)args[0];
                return new FireCommand(ship);
            })).Execute();
    }
}