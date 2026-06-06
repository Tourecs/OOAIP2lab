namespace SpaceBattle;

public sealed class RegisterIoCDependencyRotateCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Rotate", args =>
        {
            var rotatingObject = Ioc.Resolve<IRotatingObject>("Adapters.IRotatingObject", args[0]);
            return new RotateCommand(rotatingObject);
        });
    }
}
