using Xunit;

namespace OOAIP_3lab.Tests;

public class AdapterTests
{
    [Fact]
    public void MovingObjectAdapterPositionGetAndSet()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(1, 2),
            ["Velocity"] = new Vector(3, 4)
        };
        var adapter = new MovingObjectAdapter(data);
        Assert.Equal(new Vector(1, 2), adapter.Position);
        adapter.Position = new Vector(5, 6);
        Assert.Equal(new Vector(5, 6), (Vector)data["Position"]);
    }

    [Fact]
    public void MovingObjectAdapterVelocityIsReadonly()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(0, 0),
            ["Velocity"] = new Vector(3, 4)
        };
        var adapter = new MovingObjectAdapter(data);
        Assert.Equal(new Vector(3, 4), adapter.Velocity);
    }

    [Fact]
    public void AdapterRegistrationRegistersMovingObjectAdapter()
    {
        AdapterRegistration.Register();
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(0, 0),
            ["Velocity"] = new Vector(1, 0)
        };
        var moving = Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", data);
        Assert.IsType<MovingObjectAdapter>(moving);
    }

    [Fact]
    public void AdapterRegistrationThrowsForWrongType()
    {
        AdapterRegistration.Register();
        Assert.Throws<ArgumentException>(() => Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", "not a dict"));
    }
}