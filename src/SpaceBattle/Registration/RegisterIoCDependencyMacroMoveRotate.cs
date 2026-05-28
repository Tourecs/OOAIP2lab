namespace SpaceBattle;

public sealed class RegisterIoCDependencyMacroMoveRotate : ICommand
{
    public void Execute()
    {
        Ioc.Register("Macro.Move", args => new CreateMacroCommandStrategy("Move").Resolve(args));
        Ioc.Register("Macro.Rotate", args => new CreateMacroCommandStrategy("Rotate").Resolve(args));
    }
}
