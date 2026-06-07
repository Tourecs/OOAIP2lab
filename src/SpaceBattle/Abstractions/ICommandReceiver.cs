public class CommandReceiver : ICommandReceiver
{
    public void Receive(ICommand command)
    {
        command.Execute();
    }
}
