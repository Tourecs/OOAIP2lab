using Moq;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class GameTests : IDisposable
{
    private readonly Game _game;

    public GameTests()
    {
        Ioc.Clear();
        var auth = new Authorization();
        _game = new Game(auth);
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
        obj.SetVelocity(new Vector(3, 4));
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
        var ship = new ShipWithLaunch(10, 20);
        _game.Add(ship);
        _game.LaunchPhotonTorpedo(ship, 0);
        Assert.Equal(2, _game.GetAll().Count());
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
        var ship = new ShipWithLaunch(0, 0);
        _game.Add(ship);
        _game.LaunchPhotonTorpedo(ship, 0);
        _game.Update();
        var torpedo = _game.GetAll().OfType<PhotonTorpedo>().First();
        Assert.Equal(5.0, torpedo.Position.X, 10);
    }

    [Fact]
    public void GameLaunchPhotonTorpedoUnauthorizedThrows()
    {
        var denyAuth = new Mock<IAuthorization>();
        denyAuth.Setup(a => a.CanPerform(It.IsAny<IGameObject>(), "LaunchPhotonTorpedo")).Returns(false);
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var game = new Game(denyAuth.Object);
        var ship = new ShipWithLaunch(0, 0);
        Assert.Throws<UnauthorizedAccessException>(() => game.LaunchPhotonTorpedo(ship, 0));
    }

    [Fact]
    public void GameConstructorThrowsWhenAuthIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Game(null!));
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

        public void SetVelocity(Vector v) => Velocity = v;

        public void Update()
        {
            Position = Position + Velocity;
        }
    }

    private class ShipWithLaunch : IGameObject, ICanLaunchTorpedo
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Vector Position { get; private set; }
        public Vector Velocity { get; private set; } = new Vector(0, 0);
        public bool CanLaunchTorpedo => true;

        public ShipWithLaunch(double x, double y)
        {
            Position = new Vector(x, y);
        }

        public void Update()
        {
            Position = Position + Velocity;
        }
    }
}
