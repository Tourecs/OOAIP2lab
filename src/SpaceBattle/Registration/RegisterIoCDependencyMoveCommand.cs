namespace SpaceBattle;

public sealed class RegisterIoCDependencyMoveCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Move", args =>
        {
            var movingObject = Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", args[0]);
            return new MoveCommand(movingObject);
        });
    }
}
