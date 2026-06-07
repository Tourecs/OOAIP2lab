namespace OOAIP_3lab;

public class RotateCommand : ICommand
{
    private readonly IRotatingObject _rotatingObject;

    public RotateCommand(IRotatingObject rotatingObject)
    {
        _rotatingObject = rotatingObject;
    }

    public void Execute()
    {
        if (_rotatingObject.Angle == null)
        {
            throw new InvalidOperationException("Cannot get angle");
        }
        if (_rotatingObject.AngularVelocity == null)
        {
            throw new InvalidOperationException("Cannot get angular velocity");
        }
        try
        {
            _rotatingObject.Angle = _rotatingObject.Angle + _rotatingObject.AngularVelocity;
        }
        catch
        {
            throw new InvalidOperationException("Cannot change angle");
        }
    }
}