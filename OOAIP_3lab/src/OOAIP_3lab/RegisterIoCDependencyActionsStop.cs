namespace OOAIP_3lab;

public class RegisterIoCDependencyActionsStop : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Actions.Stop",
            (Func<object[], object>)(args =>
            {
                var gameObject = (IDictionary<string, object>)args[0];
                var cmdType = (string)args[1];
                return new StopCommand(gameObject, cmdType);
            })
        ).Execute();
    }
}