namespace OOAIP_3lab;

public class MoveCommand : ICommand
{
    private readonly IMovingObject _movingObject;

    public MoveCommand(IMovingObject movingObject)
    {
        _movingObject = movingObject;
    }

    public void Execute()
    {
        try
        {
            var newPosition = _movingObject.Position + _movingObject.Velocity;
            _movingObject.Position = newPosition;
        }
        catch when (_movingObject.Position == null)
        {
            throw new InvalidOperationException("Cannot get position");
        }
        catch when (_movingObject.Velocity == null)
        {
            throw new InvalidOperationException("Cannot get velocity");
        }
        catch (InvalidOperationException)
        {
            throw;
        }
    }
}