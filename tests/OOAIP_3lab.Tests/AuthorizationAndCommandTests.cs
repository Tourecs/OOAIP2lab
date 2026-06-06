using OOAIP_3lab.Commands;
using OOAIP_3lab.Game;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class AuthorizationTests
{
    [Fact]
    public void IsAuthorizedReturnsTrueForValidCredentials()
    {
        var auth = new Authorization();
        Assert.True(auth.IsAuthorized("player1", "token123"));
    }

    [Fact]
    public void IsAuthorizedReturnsFalseForEmptyUser()
    {
        var auth = new Authorization();
        Assert.False(auth.IsAuthorized("", "token123"));
    }

    [Fact]
    public void IsAuthorizedReturnsFalseForNullToken()
    {
        var auth = new Authorization();
        Assert.False(auth.IsAuthorized("player1", null!));
    }

    [Fact]
    public void AuthenticateSucceedsForValidCredentials()
    {
        var auth = new Authorization();
        auth.Authenticate("player1", "token123");
    }

    [Fact]
    public void AuthenticateThrowsForInvalidCredentials()
    {
        var auth = new Authorization();
        Assert.Throws<UnauthorizedAccessException>(() => auth.Authenticate("", "token"));
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
    public void LaunchCommandAddsTorpedoWhenAuthorized()
    {
        Ioc.Register("Authorization", _ => new Authorization());
        var game = new OOAIP_3lab.Game.Game();
        Ioc.Register("Game.Current", _ => game);
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });

        var cmd = new LaunchPhotonTorpedoCommand(10, 20, 0, "player1", "token123");
        cmd.Execute();

        Assert.Single(game.GetAll());
    }

    [Fact]
    public void LaunchCommandThrowsUnauthorizedWhenCredentialsInvalid()
    {
        Ioc.Register("Authorization", _ => new Authorization());
        Ioc.Register("Game.Current", _ => new OOAIP_3lab.Game.Game());
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });

        var cmd = new LaunchPhotonTorpedoCommand(10, 20, 0, "", "invalid");

        Assert.Throws<UnauthorizedAccessException>(() => cmd.Execute());
    }

    [Fact]
    public void LaunchCommandTorpedoMovesAfterUpdate()
    {
        Ioc.Register("Authorization", _ => new Authorization());
        var game = new OOAIP_3lab.Game.Game();
        Ioc.Register("Game.Current", _ => game);
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });

        var cmd = new LaunchPhotonTorpedoCommand(0, 0, 0, "player1", "token123");
        cmd.Execute();
        game.Update();

        var torpedo = game.GetAll().First();
        Assert.Equal(5.0, torpedo.Position.X, 10);
    }

    [Fact]
    public void LaunchCommandThrowsWhenUserIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(0, 0, 0, null!, "token"));
    }

    [Fact]
    public void LaunchCommandThrowsWhenTokenIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LaunchPhotonTorpedoCommand(0, 0, 0, "user", null!));
    }
}