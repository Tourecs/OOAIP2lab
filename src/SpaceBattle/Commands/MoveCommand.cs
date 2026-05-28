namespace SpaceBattle;

public sealed class MoveCommand : ICommand
{
    private readonly IMovingObject _movingObject;

    public MoveCommand(IMovingObject movingObject)
    {
        _movingObject = movingObject ?? throw new ArgumentNullException(nameof(movingObject));
    }

    public void Execute()
    {
        var position = _movingObject.Position ?? throw new InvalidOperationException("Position is not defined.");
        var velocity = _movingObject.Velocity ?? throw new InvalidOperationException("Velocity is not defined.");
        _movingObject.Position = position + velocity;
    }
}
