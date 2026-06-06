using OOAIP_3lab.Game;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class GameTests : IDisposable
{
    private readonly OOAIP_3lab.Game.Game _game;

    public GameTests()
    {
        Ioc.Clear();
        _game = new OOAIP_3lab.Game.Game();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void GameAddAndRetrieveGameObject()
    {
        var obj = new TestGameObject(1, 2);

        _game.Add(obj);

        Assert.Single(_game.GetAll());
    }

    [Fact]
    public void GameRemoveGameObject()
    {
        var obj = new TestGameObject(1, 2);
        _game.Add(obj);

        _game.Remove(obj);

        Assert.Empty(_game.GetAll());
    }

    [Fact]
    public void GameGetByIdReturnsCorrectObject()
    {
        var obj = new TestGameObject(1, 2);
        _game.Add(obj);

        var found = _game.GetById(obj.Id);

        Assert.Equal(obj.Id, found.Id);
    }

    [Fact]
    public void GameGetByIdThrowsWhenNotFound()
    {
        Assert.Throws<InvalidOperationException>(() => _game.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void GameUpdateMovesAllObjects()
    {
        var obj = new TestGameObject(1, 2);
        obj.Velocity = new Vector(3, 4);
        _game.Add(obj);

        _game.Update();

        Assert.Equal(4, obj.Position.X);
        Assert.Equal(6, obj.Position.Y);
    }

    [Fact]
    public void GameLaunchPhotonTorpedoAddsTorpedo()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });

        _game.LaunchPhotonTorpedo(10, 20, 0);

        Assert.Single(_game.GetAll());
    }

    [Fact]
    public void GameLaunchPhotonTorpedoCreatesMovingTorpedo()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });

        _game.LaunchPhotonTorpedo(0, 0, 0);
        _game.Update();

        var torpedo = _game.GetAll().First();
        Assert.Equal(5.0, torpedo.Position.X, 10);
    }

    private class TestGameObject : IGameObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Vector Position { get; set; }
        public Vector Velocity { get; set; } = new Vector(0, 0);

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