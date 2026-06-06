using OOAIP_3lab.Game;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class MyGameTests : IDisposable
{
    public MyGameTests()
    {
        Ioc.Clear();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void MyGameUpdateMovesGameObjects()
    {
        var repo = new TestRepository();
        var obj = new TestObj { Position = new Vector(1, 2), Velocity = new Vector(3, 4) };
        repo.Add(obj);

        var myGame = new MyGame(repo);
        myGame.Update();

        Assert.Equal(new Vector(4, 6), obj.Position);
    }

    [Fact]
    public void MyGameLaunchPhotonTorpedoAddsToRepository()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var repo = new TestRepository();
        var myGame = new MyGame(repo);

        myGame.LaunchPhotonTorpedo(10, 20, 0);

        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void MyGameLaunchTorpedoAndItMoves()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var dir = (double)args[2];
            return new PhotonTorpedo(x, y, dir, 5.0);
        });
        var repo = new TestRepository();
        var myGame = new MyGame(repo);

        myGame.LaunchPhotonTorpedo(0, 0, 0);
        myGame.Update();

        var torpedo = repo.GetAll().First();
        Assert.Equal(5.0, torpedo.Position.X, 10);
    }

    [Fact]
    public void MyGameThrowsWhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MyGame(null!));
    }

    private class TestObj : IGameObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Vector Position { get; set; } = new Vector(0, 0);
        public Vector Velocity { get; set; } = new Vector(0, 0);

        public void Update()
        {
            Position = Position + Velocity;
        }
    }

    private class TestRepository : IGameObjectRepository
    {
        private readonly List<IGameObject> _objects = new();

        public void Add(IGameObject obj) => _objects.Add(obj);
        public void Remove(IGameObject obj) => _objects.Remove(obj);
        public IEnumerable<IGameObject> GetAll() => _objects;
        public IGameObject GetById(Guid id) => _objects.First(o => o.Id == id);
    }
}