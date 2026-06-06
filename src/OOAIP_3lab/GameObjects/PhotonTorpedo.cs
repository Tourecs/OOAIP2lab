namespace OOAIP_3lab.GameObjects;

public class PhotonTorpedo : GameObject
{
    public double Direction { get; set; }
    public double Speed { get; set; }

    public PhotonTorpedo(double x, double y, double direction, double speed)
        : base(x, y, 0, 0)
    {
        Direction = direction;
        Speed = speed;
        Velocity = Vector.FromAngle(Direction, Speed);
    }

    public override void Update()
    {
        base.Update();
    }
}