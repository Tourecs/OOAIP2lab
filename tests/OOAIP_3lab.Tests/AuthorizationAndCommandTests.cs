using OOAIP_3lab.Commands;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class AuthorizationTests
{
    [Fact]
    public void PlayerCanLaunchTorpedo()
    {
        var auth = new Authorization();
        Assert.True(auth.CanPerform("player", "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void AdminCanLaunchTorpedo()
    {
        var auth = new Authorization();
        Assert.True(auth.CanPerform("admin", "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void ObserverCannotLaunchTorpedo()
    {
        var auth = new Authorization();
        Assert.False(auth.CanPerform("observer", "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void UnknownRoleCannotPerform()
    {
        var auth = new Authorization();
        Assert.False(auth.CanPerform("guest", "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void PlayerCanMove()
    {
        var auth = new Authorization();
        Assert.True(auth.CanPerform("player", "Move"));
    }

    [Fact]
    public void ObserverCanView()
    {
        var auth = new Authorization();
        Assert.True(auth.CanPerform("observer", "View"));
    }

    [Fact]
    public void CanPerformThrowsWhenRoleIsNull()
    {
        var auth = new Authorization();
        Assert.Throws<ArgumentNullException>(() => auth.CanPerform(null!, "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void CanPerformThrowsWhenActionIsEmpty()
    {
        var auth = new Authorization();
        Assert.Throws<ArgumentException>(() => auth.CanPerform("player", ""));
    }

    [Fact]
    public void GrantAddsPermissionToExistingRole()
    {
        var auth = new Authorization();
        auth.Grant("observer", "LaunchPhotonTorpedo");
        Assert.True(auth.CanPerform("observer", "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void GrantAddsPermissionToNewRole()
    {
        var auth = new Authorization();
        auth.Grant("moderator", "Kick");
        Assert.True(auth.CanPerform("moderator", "Kick"));
    }

    [Fact]
    public void RevokeRemovesPermission()
    {
        var auth = new Authorization();
        auth.Revoke("player", "LaunchPhotonTorpedo");
        Assert.False(auth.CanPerform("player", "LaunchPhotonTorpedo"));
    }

    [Fact]
    public void RevokeOnNonExistentRoleDoesNotThrow()
    {
        var auth = new Authorization();
        auth.Revoke("nonexistent", "anything");
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
        var ship = new TestGameObject(10, 20);

        var cmd = new LaunchPhotonTorpedoCommand(ship, 0, "player", game);
        cmd.Execute();

        Assert.Single(game.GetAll().OfType<PhotonTorpedo>());
    }

    [Fact]
    public void LaunchCommandThrowsWhenShipIsNull()
    {
        var auth = new Authorization();
        var game = new Game(auth);
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(null!, 0, "player", game));
    }

    [Fact]
    public void LaunchCommandThrowsWhenGameIsNull()
    {
        var ship = new TestGameObject(0, 0);
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(ship, 0, "player", null!));
    }

    [Fact]
    public void LaunchCommandThrowsWhenRoleIsNull()
    {
        var auth = new Authorization();
        var game = new Game(auth);
        var ship = new TestGameObject(0, 0);
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(ship, 0, null!, game));
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
        var auth = new Authorization();
        auth.Revoke("player", "LaunchPhotonTorpedo");
        var game = new Game(auth);

        var ship = new TestGameObject(0, 0);
        var cmd = new LaunchPhotonTorpedoCommand(ship, 0, "player", game);
        Assert.Throws<UnauthorizedAccessException>(() => cmd.Execute());
    }

    private class TestGameObject : IGameObject
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Vector Position { get; private set; }
        public Vector Velocity { get; private set; } = new Vector(0, 0);

        public TestGameObject(double x, double y)
        {
            Position = new Vector(x, y);
        }

        public void Update()
        {
            Position = Position + Velocity;
        }
    }
}