using Moq;
using OOAIP_3lab.Commands;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class AuthorizationTests
{
    [Fact]
    public void CanPerformReturnsTrueForLaunchTorpedoWhenObjectHasCapability()
    {
        var auth = new Authorization();
        var ship = new Mock<ICanLaunchTorpedo>();
        ship.SetupGet(s => s.CanLaunchTorpedo).Returns(true);
        var obj = ship.As<IGameObject>();
        Assert.True(auth.CanPerform(obj.Object, "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void CanPerformReturnsFalseWhenObjectLacksCapability()
    {
        var auth = new Authorization();
        var obj = Mock.Of<IGameObject>();
        Assert.False(auth.CanPerform(obj, "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void CanPerformReturnsFalseForUnknownAction()
    {
        var auth = new Authorization();
        var ship = new Mock<ICanLaunchTorpedo>();
        ship.SetupGet(s => s.CanLaunchTorpedo).Returns(true);
        var obj = ship.As<IGameObject>();
        Assert.False(auth.CanPerform(obj.Object, "UnknownAction"));
    }

    [Fact]
    public void CanPerformThrowsWhenObjectIsNull()
    {
        var auth = new Authorization();
        Assert.Throws<ArgumentNullException>(() => auth.CanPerform(null!, "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void CanPerformThrowsWhenActionIsEmpty()
    {
        var auth = new Authorization();
        var obj = Mock.Of<IGameObject>();
        Assert.Throws<ArgumentException>(() => auth.CanPerform(obj, ""));
    }
}

public sealed class LaunchPhotonTorpedoCommandTests : IDisposable
{
    public LaunchPhotonTorpedoCommandTests()
    {
        Ioc.Clear();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void LaunchCommandAddsTorpedoFromShipPosition()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var auth = new Authorization();
        var game = new Game(auth);

        var ship = new ShipWithLaunchCapability(10, 20);
        var cmd = new LaunchPhotonTorpedoCommand(ship, 0, game);
        cmd.Execute();

        Assert.Single(game.GetAll().OfType<PhotonTorpedo>());
    }

    [Fact]
    public void LaunchCommandThrowsWhenShipIsNull()
    {
        var auth = new Authorization();
        var game = new Game(auth);
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(null!, 0, game));
    }

    [Fact]
    public void LaunchCommandThrowsWhenGameIsNull()
    {
        var ship = new ShipWithLaunchCapability(0, 0);
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(ship, 0, null!));
    }

    [Fact]
    public void LaunchCommandUnauthorizedThrows()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var denyAuth = new Mock<IAuthorization>();
        denyAuth.Setup(a => a.CanPerform(It.IsAny<IGameObject>(), "LaunchPhotonTorpedo")).Returns(false);
        var game = new Game(denyAuth.Object);

        var ship = new ShipWithLaunchCapability(0, 0);
        var cmd = new LaunchPhotonTorpedoCommand(ship, 0, game);
        Assert.Throws<UnauthorizedAccessException>(() => cmd.Execute());
    }

    private class ShipWithLaunchCapability : IGameObject, ICanLaunchTorpedo
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Vector Position { get; private set; }
        public Vector Velocity { get; private set; } = new Vector(0, 0);
        public bool CanLaunchTorpedo => true;

        public ShipWithLaunchCapability(double x, double y)
        {
            Position = new Vector(x, y);
        }

        public void Update()
        {
            Position = Position + Velocity;
        }
    }
}
