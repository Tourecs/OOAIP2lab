namespace OOAIP_3lab;

public class RegisterIoCDependencyMoveCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
            (Func<object[], object>)(args =>
            {
                var obj = (IDictionary<string, object>)args[0];
                var movingObj = Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", obj);
                return new MoveCommand(movingObj);
            })).Execute();
    }
}