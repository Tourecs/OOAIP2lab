namespace SpaceBattle;

public sealed class RegisterIoCDependencyMacroCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Macro", args =>
        {
            if (args.Length == 1 && args[0] is ICommand[] array)
            {
                return new MacroCommand(array);
            }

            return new MacroCommand(args.Cast<ICommand>().ToArray());
        });
    }
}
