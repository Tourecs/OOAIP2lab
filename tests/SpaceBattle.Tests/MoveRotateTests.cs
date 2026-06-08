using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
public sealed class MoveRotateTests
{
    [Fact]
    public void MoveChangesPositionByVelocity()
    {
        var movingObject = new Mock<IMovingObject>();
        var position = new Vector(12, 5);
        movingObject.SetupProperty(x => x.Position, position);
        movingObject.SetupGet(x => x.Velocity).Returns(new Vector(-4, 1));

        new MoveCommand(movingObject.Object).Execute();

        Assert.Equal(new Vector(8, 6), movingObject.Object.Position);
    }

    [Fact]
    public void MoveThrowsWhenPositionCannotBeRead()
    {
        var movingObject = new Mock<IMovingObject>();
        movingObject.SetupGet(x => x.Position).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new MoveCommand(movingObject.Object).Execute());
    }

    [Fact]
    public void MoveThrowsWhenVelocityCannotBeRead()
    {
        var movingObject = new Mock<IMovingObject>();
        movingObject.SetupGet(x => x.Position).Returns(new Vector(1, 2));
        movingObject.SetupGet(x => x.Velocity).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new MoveCommand(movingObject.Object).Execute());
    }

    [Fact]
    public void MoveThrowsWhenPositionCannotBeChanged()
    {
        var movingObject = new Mock<IMovingObject>();
        movingObject.SetupGet(x => x.Position).Returns(new Vector(1, 2));
        movingObject.SetupGet(x => x.Velocity).Returns(new Vector(1, 1));
        movingObject.SetupSet(x => x.Position = It.IsAny<Vector>()).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new MoveCommand(movingObject.Object).Execute());
    }

    [Fact]
    public void RotateChangesAngleByAngularVelocity()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.SetupProperty(x => x.Angle, new Angle(1, 8));
        rotatingObject.SetupGet(x => x.AngularVelocity).Returns(new Angle(1, 8));

        new RotateCommand(rotatingObject.Object).Execute();

        Assert.Equal(new Angle(2, 8), rotatingObject.Object.Angle);
    }

    [Fact]
    public void RotateThrowsWhenAngleCannotBeRead()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.SetupGet(x => x.Angle).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new RotateCommand(rotatingObject.Object).Execute());
    }

    [Fact]
    public void RotateThrowsWhenAngularVelocityCannotBeRead()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.SetupGet(x => x.Angle).Returns(new Angle(1, 8));
        rotatingObject.SetupGet(x => x.AngularVelocity).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new RotateCommand(rotatingObject.Object).Execute());
    }

    [Fact]
    public void RotateThrowsWhenAngleCannotBeChanged()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.SetupGet(x => x.Angle).Returns(new Angle(1, 8));
        rotatingObject.SetupGet(x => x.AngularVelocity).Returns(new Angle(1, 8));
        rotatingObject.SetupSet(x => x.Angle = It.IsAny<Angle>()).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new RotateCommand(rotatingObject.Object).Execute());
    }

    [Fact]
    public void MoveCommand_Constructor_Throws_WhenObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MoveCommand(null!));
    }

    [Fact]
    public void RotateCommand_Constructor_Throws_WhenObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RotateCommand(null!));
    }
    [Fact]
    public void MoveCommand_Execute_Throws_WhenPositionIsNull()
    {
        var movingObject = new Mock<IMovingObject>();

        movingObject
            .SetupGet(x => x.Position)
            .Returns((Vector)null!);

        movingObject
            .SetupGet(x => x.Velocity)
            .Returns(new Vector(1, 1));

        var command = new MoveCommand(movingObject.Object);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void MoveCommand_Execute_Throws_WhenVelocityIsNull()
    {
        var movingObject = new Mock<IMovingObject>();

        movingObject
            .SetupGet(x => x.Position)
            .Returns(new Vector(1, 1));

        movingObject
            .SetupGet(x => x.Velocity)
            .Returns((Vector)null!);

        var command = new MoveCommand(movingObject.Object);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void RotateCommand_Execute_Throws_WhenAngleIsNull()
    {
        var rotatingObject = new Mock<IRotatingObject>();

        rotatingObject
            .SetupGet(x => x.Angle)
            .Returns((Angle)null!);

        rotatingObject
            .SetupGet(x => x.AngularVelocity)
            .Returns(new Angle(1));

        var command = new RotateCommand(rotatingObject.Object);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void RotateCommand_Execute_Throws_WhenAngularVelocityIsNull()
    {
        var rotatingObject = new Mock<IRotatingObject>();

        rotatingObject
            .SetupGet(x => x.Angle)
            .Returns(new Angle(1));

        rotatingObject
            .SetupGet(x => x.AngularVelocity)
            .Returns((Angle)null!);

        var command = new RotateCommand(rotatingObject.Object);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }
}
