namespace OOAIP_3lab;

public class CeateMacroCommandStrategy
{
    public static ICommand Create(ICommand cmd, ICommand send)
    {
        return new MacroCommand(new ICommand[] { cmd, send });
    }
}