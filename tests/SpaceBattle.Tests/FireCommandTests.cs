using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
public sealed class FireCommandTests : IDisposable
{
    public FireCommandTests()
    {
        Ioc.Clear();
        new RegisterIoCDependencyGameRegistry().Execute();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void ReceiveCallsExecuteOnCommand()
    {
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(c => c.Execute()).Verifiable();
        var receiver = new CommandReceiver();

        receiver.Receive(mockCommand.Object);

        mockCommand.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void FireCommandCreatesTorpedoAndStartsMove()
    {
        var ship = new Dictionary<string, object>();
        var shipPos = new Vector(0, 0);
        var shipAngle = new Angle(0); // 0° = right

        Ioc.Register("Adapters.IMovingObject",
            (Func<object[], object>)(args => new MockMovingObject(shipPos)));
        Ioc.Register("Adapters.IRotatingObject",
            (Func<object[], object>)(args => new MockRotatingObject(shipAngle)));

        var addedObjects = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Register("Game.Registry.Add",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var obj = (IDictionary<string, object>)args[1];
                addedObjects[id] = obj;
                return new EmptyCommand();
            }));

        bool startCalled = false;
        Ioc.Register("Actions.Start",
            (Func<object[], object>)(args =>
            {
                startCalled = true;
                Assert.Equal("Move", args[1]);
                return new EmptyCommand();
            }));

        var fireCommand = new FireCommand(ship);
        fireCommand.Execute();

        Assert.True(startCalled);
        Assert.Single(addedObjects);
        var torpedo = addedObjects.Values.First();
        Assert.Equal(shipPos, torpedo["Position"]);
        Assert.Equal(new Vector(1, 0), torpedo["Velocity"]);
        Assert.IsType<CommandReceiver>(torpedo["Receiver"]);
    }

    [Fact]
    public void FireCommandThrowsWhenShipAngleIsNull()
    {
        var ship = new Dictionary<string, object>();
        var shipPos = new Vector(0, 0);

        Ioc.Register("Adapters.IMovingObject",
            (Func<object[], object>)(args => new MockMovingObject(shipPos)));
        Ioc.Register("Adapters.IRotatingObject",
            (Func<object[], object>)(args => new MockRotatingObject(null)));

        var fireCommand = new FireCommand(ship);
        Assert.Throws<InvalidOperationException>(() => fireCommand.Execute());
    }

    [Theory]
    [InlineData(0, 1, 0)]    // 0°  → (1, 0)
    [InlineData(2, 0, 1)]    // 90° (2/8) → (0, 1)
    [InlineData(4, -1, 0)]   // 180° (4/8) → (-1, 0)
    [InlineData(6, 0, -1)]   // 270° (6/8) → (0, -1)
    public void FireCommandComputesVelocityDirection(int numerator, int expectedVx, int expectedVy)
    {
        var ship = new Dictionary<string, object>();
        var shipPos = new Vector(0, 0);
        var shipAngle = new Angle(numerator); // denominator defaults to 8

        Ioc.Register("Adapters.IMovingObject",
            (Func<object[], object>)(args => new MockMovingObject(shipPos)));
        Ioc.Register("Adapters.IRotatingObject",
            (Func<object[], object>)(args => new MockRotatingObject(shipAngle)));

        var addedObjects = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Register("Game.Registry.Add",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var obj = (IDictionary<string, object>)args[1];
                addedObjects[id] = obj;
                return new EmptyCommand();
            }));

        Ioc.Register("Actions.Start",
            (Func<object[], object>)(args => new EmptyCommand()));

        var fireCommand = new FireCommand(ship);
        fireCommand.Execute();

        Assert.Single(addedObjects);
        var torpedo = addedObjects.Values.First();
        var velocity = (Vector)torpedo["Velocity"];
        Assert.Equal(new Vector(expectedVx, expectedVy), velocity);
    }

    [Fact]
    public void RegisterIoCDependencyFireCommandRegistersAndCreatesFireCommand()
    {
        new RegisterIoCDependencyFireCommand().Execute();

        var ship = new Dictionary<string, object>();
        var fireCommand = Ioc.Resolve<ICommand>("Commands.Fire", ship);

        Assert.NotNull(fireCommand);
        Assert.IsType<FireCommand>(fireCommand);
    }
}

internal class MockRotatingObject : IRotatingObject
{
    public Angle Angle { get; set; }
    public Angle AngularVelocity { get; }

    public MockRotatingObject(Angle? angle)
    {
        Angle = angle!; 
        AngularVelocity = null!;
    }
}

internal class MockRotatingObject : IRotatingObject
{
    public Angle Angle { get; set; }
    public Angle AngularVelocity { get; } = null!; 
    public MockRotatingObject(Angle? angle) => Angle = angle!;
}
