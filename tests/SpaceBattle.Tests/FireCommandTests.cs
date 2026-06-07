using SpaceBattle;
using Xunit;
using Moq;

namespace SpaceBattle.Tests;

[Collection("Sequential")]
public class FireCommandTests : IDisposable
{
    private readonly int _previousDenominator;

    public FireCommandTests()
    {
        Ioc.Clear();
        _previousDenominator = Angle.Denominator;
        Angle.Denominator = 8;
        new RegisterIoCDependencyGameRegistry().Execute();
    }

    public void Dispose()
    {
        Angle.Denominator = _previousDenominator;
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
        var shipAngle = new Angle(0, 360);

        Ioc.Register("Adapters.IMovingObject", args => new MockMovingObject(shipPos));
        Ioc.Register("Adapters.IRotatingObject", args => new MockRotatingObject(shipAngle));

        var addedObjects = new Dictionary<Guid, IDictionary<string, object>>();
        
        // Override Game.Registry.Add to capture objects
        var originalAdd = Ioc.Resolve<object>("Game.Registry.Add", Guid.NewGuid(), new Dictionary<string, object>());
        Ioc.Register("Game.Registry.Add", args =>
        {
            var id = (Guid)args[0];
            var obj = (IDictionary<string, object>)args[1];
            addedObjects[id] = obj;
            return new EmptyCommand();
        });

        bool startCalled = false;
        Ioc.Register("Actions.Start", args =>
        {
            startCalled = true;
            Assert.Equal("Move", args[1]);
            return new EmptyCommand();
        });

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

        Ioc.Register("Adapters.IMovingObject", args => new MockMovingObject(shipPos));
        Ioc.Register("Adapters.IRotatingObject", args => new MockRotatingObject(null));

        var fireCommand = new FireCommand(ship);
        Assert.Throws<InvalidOperationException>(() => fireCommand.Execute());
    }

    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(90, 0, 1)]
    [InlineData(180, -1, 0)]
    [InlineData(270, 0, -1)]
    public void FireCommandComputesVelocityDirection(int degrees, int expectedVx, int expectedVy)
    {
        var ship = new Dictionary<string, object>();
        var shipPos = new Vector(0, 0);
        var shipAngle = new Angle(degrees, 360);

        Ioc.Register("Adapters.IMovingObject", args => new MockMovingObject(shipPos));
        Ioc.Register("Adapters.IRotatingObject", args => new MockRotatingObject(shipAngle));

        var addedObjects = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Register("Game.Registry.Add", args =>
        {
            var id = (Guid)args[0];
            var obj = (IDictionary<string, object>)args[1];
            addedObjects[id] = obj;
            return new EmptyCommand();
        });

        Ioc.Register("Actions.Start", args => new EmptyCommand());

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
        var registerCommand = new RegisterIoCDependencyFireCommand();
        registerCommand.Execute();

        var ship = new Dictionary<string, object>();
        var fireCommand = Ioc.Resolve<ICommand>("Commands.Fire", ship);

        Assert.NotNull(fireCommand);
        Assert.IsType<FireCommand>(fireCommand);
    }
}

internal class MockMovingObject : IMovingObject
{
    public Vector Position { get; set; }
    public Vector Velocity { get; }

    public MockMovingObject(Vector pos, Vector? vel = null)
    {
        Position = pos;
        Velocity = vel ?? new Vector(0, 0);
    }
}

internal class MockRotatingObject : IRotatingObject
{
    public Angle? Angle { get; set; }
    public Angle? AngularVelocity { get; }

    public MockRotatingObject(Angle? angle) => Angle = angle;
}
