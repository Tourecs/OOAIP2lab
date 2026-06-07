namespace SpaceBattle;

public sealed class CommandReceiver : ICommandReceiver
{
    public void Receive(ICommand command)
    {
        command.Execute();
    }
}