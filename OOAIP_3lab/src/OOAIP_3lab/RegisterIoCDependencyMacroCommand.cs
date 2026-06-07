namespace OOAIP_3lab;

public class RegisterIoCDependencyMacroCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Macro.Create",
            (Func<object[], object>)(args =>
            {
                var commands = (ICommand[])args[0];
                return new MacroCommand(commands);
            })).Execute();
    }
}