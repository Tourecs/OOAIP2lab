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

    [Fact]
    public void RotatingObjectAdapterAngleGetAndSet()
    {
        Angle.Denominator = 360;
        var data = new Dictionary<string, object>
        {
            ["Angle"] = new Angle(90, 360),
            ["AngularVelocity"] = new Angle(5, 360)
        };
        var adapter = new RotatingObjectAdapter(data);
        Assert.Equal(90, adapter.Angle!.Numerator);
        adapter.Angle = new Angle(180, 360);
        Assert.Equal(180, ((Angle)data["Angle"]).Numerator);
    }

    [Fact]
    public void AdapterRegistrationRegistersRotatingObjectAdapter()
    {
        Angle.Denominator = 360;
        AdapterRegistration.Register();
        var data = new Dictionary<string, object>
        {
            ["Angle"] = new Angle(90, 360),
            ["AngularVelocity"] = new Angle(5, 360)
        };
        var rotating = Ioc.Resolve<IRotatingObject>("Adapters.IRotatingObject", data);
        Assert.IsType<RotatingObjectAdapter>(rotating);
    }

    [Fact]
    public void RotatingObjectAdapterReturnsNullWhenAngleNotPresent()
    {
        Angle.Denominator = 360;
        var data = new Dictionary<string, object>();
        var adapter = new RotatingObjectAdapter(data);
        Assert.Null(adapter.Angle);
        Assert.Null(adapter.AngularVelocity);
    }

    [Fact]
    public void AdapterRegistrationThrowsForRotatingWrongType()
    {
        AdapterRegistration.Register();
        Assert.Throws<ArgumentException>(() => Ioc.Resolve<IRotatingObject>("Adapters.IRotatingObject", "not a dict"));
    }
}