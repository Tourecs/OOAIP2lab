using Xunit;

namespace OOAIP_3lab.Tests;

public class MoveCommandTests
{
    [Fact]
    public void MoveCommandChangesPosition()
    {
        var movingObj = new MockMovingObject(new Vector(0, 0), new Vector(1, 2));
        var cmd = new MoveCommand(movingObj);
        cmd.Execute();
        Assert.Equal(new Vector(1, 2), movingObj.Position);
    }

    [Fact]
    public void MoveCommandThrowsWhenPositionIsNull()
    {
        var movingObj = new MockMovingObjectWithNullPosition();
        var cmd = new MoveCommand(movingObj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void MoveCommandThrowsWhenVelocityIsNull()
    {
        var movingObj = new MockMovingObjectNullVelocity();
        var cmd = new MoveCommand(movingObj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void MoveCommandRethrowsInvalidOperationException()
    {
        var movingObj = new MockMovingObjectThrowsIOE();
        var cmd = new MoveCommand(movingObj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    private class MockMovingObject : IMovingObject
    {
        public Vector Position { get; set; }
        public Vector Velocity { get; }
        public MockMovingObject(Vector pos, Vector vel) { Position = pos; Velocity = vel; }
    }

    private class MockMovingObjectWithNullPosition : IMovingObject
    {
        public Vector Position { get => null!; set { } }
        public Vector Velocity => new Vector(1, 0);
    }

    private class MockMovingObjectNullVelocity : IMovingObject
    {
        public Vector Position { get; set; } = new Vector(0, 0);
        public Vector Velocity => null!;
    }

    private class MockMovingObjectThrowsIOE : IMovingObject
    {
        public Vector Position { get => throw new InvalidOperationException(); set { } }
        public Vector Velocity => new Vector(1, 0);
    }
}