namespace OOAIP_3lab;

public class RegisterIoCDependencyRotateCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Commands.Rotate",
            (Func<object[], object>)(args =>
            {
                var obj = (IDictionary<string, object>)args[0];
                var rotatingObj = Ioc.Resolve<IRotatingObject>("Adapters.IRotatingObject", obj);
                return new RotateCommand(rotatingObj);
            })).Execute();
    }
}