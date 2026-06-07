using Xunit;

namespace OOAIP_3lab.Tests;

public class RotateCommandTests
{
    public RotateCommandTests() { Angle.Denominator = 360; }

    [Fact]
    public void RotateCommandChangesAngle()
    {
        var obj = new MockRotatingObject(new Angle(10, 360), new Angle(5, 360));
        var cmd = new RotateCommand(obj);
        cmd.Execute();
        Assert.Equal(15, obj.Angle!.Numerator);
    }

    [Fact]
    public void RotateCommandThrowsWhenAngleIsNull()
    {
        var obj = new MockRotatingObject(null, new Angle(5, 360));
        var cmd = new RotateCommand(obj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void RotateCommandThrowsWhenAngularVelocityIsNull()
    {
        var obj = new MockRotatingObject(new Angle(10, 360), null);
        var cmd = new RotateCommand(obj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void RotateCommandThrowsWhenCannotSetAngle()
    {
        var obj = new MockRotatingObjectThrowsOnSet(new Angle(0, 360), new Angle(5, 360));
        var cmd = new RotateCommand(obj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    private class MockRotatingObject : IRotatingObject
    {
        public Angle? Angle { get; set; }
        public Angle? AngularVelocity { get; }
        public MockRotatingObject(Angle? angle, Angle? av) { Angle = angle; AngularVelocity = av; }
    }

    private class MockRotatingObjectThrowsOnSet : IRotatingObject
    {
        public Angle? Angle { get => _angle; set => throw new Exception("Cannot set"); }
        private readonly Angle? _angle;
        public Angle? AngularVelocity { get; }
        public MockRotatingObjectThrowsOnSet(Angle? angle, Angle? av) { _angle = angle; AngularVelocity = av; }
    }
}