namespace OOAIP_3lab.GameObjects;

public abstract class GameObject : IGameObject
{
    public Guid Id { get; set; }
    public Vector Position { get; set; }
    public Vector Velocity { get; set; }

    protected GameObject()
    {
        Id = Guid.NewGuid();
        Position = new Vector(0, 0);
        Velocity = new Vector(0, 0);
    }

    protected GameObject(double x, double y, double vx, double vy)
    {
        Id = Guid.NewGuid();
        Position = new Vector(x, y);
        Velocity = new Vector(vx, vy);
    }

    public virtual void Update()
    {
        Position = Position + Velocity;
    }
}