namespace SpaceBattle;

public sealed class RotateCommand : ICommand
{
    private readonly IRotatingObject _rotatingObject;

    public RotateCommand(IRotatingObject rotatingObject)
    {
        _rotatingObject = rotatingObject ?? throw new ArgumentNullException(nameof(rotatingObject));
    }

    public void Execute()
    {
        var angle = _rotatingObject.Angle ?? throw new InvalidOperationException("Angle is not defined.");
        var angularVelocity = _rotatingObject.AngularVelocity ?? throw new InvalidOperationException("Angular velocity is not defined.");
        _rotatingObject.Angle = angle + angularVelocity;
    }
}
