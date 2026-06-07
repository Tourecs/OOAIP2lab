namespace OOAIP_3lab;

public class CommandReceiver : ICommandReceiver
{
    public void Receive(ICommand command)
    {
        command.Execute();
    }
}