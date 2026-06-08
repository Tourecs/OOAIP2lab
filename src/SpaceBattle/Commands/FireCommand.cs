[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SpaceBattle;
public sealed class FireCommand : ICommand
{
    private readonly IDictionary<string, object> _ship;

    public FireCommand(IDictionary<string, object> ship)
    {
        _ship = ship;
    }

    public void Execute()
    {
        var movingShip = Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", _ship);
        var shipPos = movingShip.Position;

        var rotatingShip = Ioc.Resolve<IRotatingObject>("Adapters.IRotatingObject", _ship);
        var angle = rotatingShip.Angle
            ?? throw new InvalidOperationException("Ship angle is null");

        double angleRad = angle;
        int vx = (int)Math.Round(Math.Cos(angleRad));
        int vy = (int)Math.Round(Math.Sin(angleRad));
        var velocity = new Vector(vx, vy);

        var torpedo = new Dictionary<string, object>
        {
            ["Position"] = shipPos,
            ["Velocity"] = velocity,
            ["Receiver"] = new CommandReceiver()
        };
        var torpedoId = Guid.NewGuid();

        Ioc.Resolve<ICommand>("Game.Registry.Add", torpedoId, torpedo).Execute();

        Ioc.Resolve<ICommand>("Actions.Start", torpedo, "Move").Execute();
    }
}
