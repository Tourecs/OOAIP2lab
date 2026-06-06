using Moq;
using OOAIP_3lab.Commands;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class AuthorizationTests
{
    [Fact]
    public void CanPerformReturnsTrueForKnownAction()
    {
        var auth = new Authorization();
        var obj = Mock.Of<IGameObject>();
        Assert.True(auth.CanPerform(obj, "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void CanPerformReturnsFalseForUnknownAction()
    {
        var auth = new Authorization();
        var obj = Mock.Of<IGameObject>();
        Assert.False(auth.CanPerform(obj, "UnknownAction"));
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
        Ioc.Register("Authorization", _ => new Authorization());
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var game = new Game();
        Ioc.Register("Game.Current", _ => game);

        var ship = Mock.Of<IGameObject>(s => s.Position == new Vector(10, 20));
        var cmd = new LaunchPhotonTorpedoCommand(ship, 0);
        cmd.Execute();

        var torpedo = game.GetAll().OfType<PhotonTorpedo>().First();
        Assert.Equal(10, torpedo.Position.X);
        Assert.Equal(20, torpedo.Position.Y);
    }

    [Fact]
    public void LaunchCommandThrowsWhenShipIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(null!, 0));
    }

    [Fact]
    public void LaunchCommandUnauthorizedThrows()
    {
        var denyAuth = new Mock<IAuthorization>();
        denyAuth.Setup(a => a.CanPerform(It.IsAny<IGameObject>(), "LaunchPhotonTorpedo")).Returns(false);
        Ioc.Register("Authorization", _ => denyAuth.Object);
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var game = new Game();
        Ioc.Register("Game.Current", _ => game);

        var ship = Mock.Of<IGameObject>(s => s.Position == new Vector(0, 0));
        var cmd = new LaunchPhotonTorpedoCommand(ship, 0);
        Assert.Throws<UnauthorizedAccessException>(() => cmd.Execute());
    }
}
