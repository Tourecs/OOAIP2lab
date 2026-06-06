namespace OOAIP_3lab.GameObjects;

public class PhotonTorpedo : IGameObject
{
    public Guid Id { get; } = Guid.NewGuid();
    public Vector Position { get; private set; }
    public Vector Velocity { get; private set; }
    public double Direction { get; }
    public double Speed { get; }

    public PhotonTorpedo(double x, double y, double direction, double speed)
    {
        Position = new Vector(x, y);
        Direction = direction;
        Speed = speed;
        Velocity = Vector.FromAngle(Direction, Speed);
    }

    public void Update()
    {
        Position = Position + Velocity;
    }
}
