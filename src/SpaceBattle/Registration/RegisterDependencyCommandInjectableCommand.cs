namespace SpaceBattle;

public sealed class RegisterDependencyCommandInjectableCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.CommandInjectable", _ => new CommandInjectableCommand());
        Ioc.Register("Commands.CommadInjectable", _ => new CommandInjectableCommand());
    }
}
