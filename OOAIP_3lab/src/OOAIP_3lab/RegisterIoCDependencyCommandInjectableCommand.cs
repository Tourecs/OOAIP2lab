namespace OOAIP_3lab;

public class RegisterIoCDependencyCommandInjectableCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Commands.CommandInjectable",
            (Func<object[], object>)(_ => new CommandInjectableCommand())).Execute();
    }
}