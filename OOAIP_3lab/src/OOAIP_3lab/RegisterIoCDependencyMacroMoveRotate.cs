namespace OOAIP_3lab;

public class RegisterIoCDependencyMacroMoveRotate : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Macro.Move",
            (Func<object[], object>)(args =>
            {
                var cmd = (ICommand)args[0];
                var send = (ICommand)args[1];
                return CeateMacroCommandStrategy.Create(cmd, send);
            })).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Macro.Rotate",
            (Func<object[], object>)(args =>
            {
                var cmd = (ICommand)args[0];
                var send = (ICommand)args[1];
                return CeateMacroCommandStrategy.Create(cmd, send);
            })).Execute();
    }
}