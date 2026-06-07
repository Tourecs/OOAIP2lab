using Moq;
using Xunit;

namespace OOAIP_3lab.Tests;

public class FireCommandTests
{
    public FireCommandTests()
    {
        Angle.Denominator = 360;
    }

    [Fact]
    public void FireCommandCreatesTorpedoAndStartsMove()
    {
        var ship = new Dictionary<string, object>();
        var shipPos = new Vector(0, 0);
        var shipAngle = new Angle(0, 360);

        var mockMoving = new Mock<IMovingObject>();
        mockMoving.Setup(m => m.Position).Returns(shipPos);
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IMovingObject",
            (object[] args) => mockMoving.Object).Execute();

        var mockRotating = new Mock<IRotatingObject>();
        mockRotating.Setup(m => m.Angle).Returns(shipAngle);
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IRotatingObject",
            (object[] args) => mockRotating.Object).Execute();

        var addedObjects = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry.Add",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var obj = (IDictionary<string, object>)args[1];
                addedObjects[id] = obj;
                return new EmptyCommand();
            })).Execute();

        bool startCalled = false;
        Ioc.Resolve<ICommand>("IoC.Register", "Actions.Start",
            (Func<object[], object>)(args =>
            {
                startCalled = true;
                Assert.Equal("Move", args[1]);
                return new EmptyCommand();
            })).Execute();

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
        var mockMoving = new Mock<IMovingObject>();
        mockMoving.Setup(m => m.Position).Returns(new Vector(0, 0));
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IMovingObject",
            (object[] args) => mockMoving.Object).Execute();

        var mockRotating = new Mock<IRotatingObject>();
        mockRotating.Setup(m => m.Angle).Returns((Angle?)null);
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IRotatingObject",
            (object[] args) => mockRotating.Object).Execute();

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
        var shipAngle = new Angle(degrees, 360);

        var mockMoving = new Mock<IMovingObject>();
        mockMoving.Setup(m => m.Position).Returns(new Vector(0, 0));
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IMovingObject",
            (object[] args) => mockMoving.Object).Execute();

        var mockRotating = new Mock<IRotatingObject>();
        mockRotating.Setup(m => m.Angle).Returns(shipAngle);
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IRotatingObject",
            (object[] args) => mockRotating.Object).Execute();

        var addedObjects = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry.Add",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var obj = (IDictionary<string, object>)args[1];
                addedObjects[id] = obj;
                return new EmptyCommand();
            })).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Actions.Start",
            (Func<object[], object>)(args => new EmptyCommand())).Execute();

        var fireCommand = new FireCommand(ship);
        fireCommand.Execute();

        Assert.Single(addedObjects);
        var torpedo = addedObjects.Values.First();
        var velocity = (Vector)torpedo["Velocity"];
        Assert.Equal(new Vector(expectedVx, expectedVy), velocity);
    }

    [Fact]
    public void RegisterIoCDependencyFireCommandRegistersAndCreates()
    {
        new RegisterIoCDependencyFireCommand().Execute();
        var ship = new Dictionary<string, object>();
        var fireCommand = Ioc.Resolve<ICommand>("Commands.Fire", ship);
        Assert.NotNull(fireCommand);
        Assert.IsType<FireCommand>(fireCommand);
    }

    [Fact]
    public void CommandReceiverReceivesCommand()
    {
        var mockCommand = new Mock<ICommand>();
        var receiver = new CommandReceiver();
        receiver.Receive(mockCommand.Object);
        mockCommand.Verify(c => c.Execute(), Times.Once);
    }
}