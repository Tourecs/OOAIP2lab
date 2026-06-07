namespace OOAIP_3lab;

public interface ICommand
{
    void Execute();
}

public interface ICommandReceiver
{
    void Receive(ICommand command);
}

public interface ICommandInjectable
{
    void Inject(ICommand command);
}

public interface IMovingObject
{
    Vector Position { get; set; }
    Vector Velocity { get; }
}

public interface IRotatingObject
{
    Angle? Angle { get; set; }
    Angle? AngularVelocity { get; }
}