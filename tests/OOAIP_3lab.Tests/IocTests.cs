using OOAIP_3lab.Commands;
using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class IocTests : IDisposable
{
    public IocTests()
    {
        Ioc.Clear();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void RegisterPhotonTorpedoDependencyAllowsResolving()
    {
        new RegisterIoCDependencyPhotonTorpedo().Execute();
        var torpedo = Ioc.Resolve<IGameObject>("GameObjects.PhotonTorpedo", 0.0, 0.0, 0.0);
        Assert.IsType<PhotonTorpedo>(torpedo);
    }

    [Fact]
    public void RegisterAuthorizationDependencyAllowsResolving()
    {
        new RegisterIoCDependencyAuthorization().Execute();
        var auth = Ioc.Resolve<IAuthorization>("Authorization");
        Assert.IsType<Authorization>(auth);
    }

    [Fact]
    public void RegisterGameDependencyAllowsResolving()
    {
        new RegisterIoCDependencyGame().Execute();
        var game = Ioc.Resolve<Game>("Game.Current");
        Assert.IsType<Game>(game);
    }

    [Fact]
    public void RegisterLaunchCommandDependencyAllowsResolving()
    {
        new RegisterIoCDependencyLaunchPhotonTorpedoCommand().Execute();
        var ship = Mock.Of<IGameObject>();
        var cmd = Ioc.Resolve<ICommand>("Commands.LaunchPhotonTorpedo", ship, 0.0);
        Assert.IsType<LaunchPhotonTorpedoCommand>(cmd);
    }

    [Fact]
    public void RegisterMoveCommandDependencyAllowsResolving()
    {
        new RegisterIoCDependencyMoveCommand().Execute();
        var obj = Mock.Of<IGameObject>();
        var cmd = Ioc.Resolve<ICommand>("Commands.Move", obj);
        Assert.IsType<MoveCommand>(cmd);
    }

    [Fact]
    public void MoveCommandMovesGameObject()
    {
        var obj = new TestGameObject { Position = new Vector(1, 2), Velocity = new Vector(3, 4) };
        var cmd = new MoveCommand(obj);
        cmd.Execute();
        Assert.Equal(new Vector(4, 6), obj.Position);
    }

    [Fact]
    public void MoveCommandThrowsWhenGameObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MoveCommand(null!));
    }

    [Fact]
    public void RegisterMoveCommandAndExecuteThroughIoC()
    {
        new RegisterIoCDependencyMoveCommand().Execute();
        var obj = new TestGameObject { Position = new Vector(10, 20), Velocity = new Vector(1, 2) };
        var cmd = Ioc.Resolve<ICommand>("Commands.Move", obj);
        cmd.Execute();
        Assert.Equal(new Vector(11, 22), obj.Position);
    }

    [Fact]
    public void FullLaunchFlowWithIoC()
    {
        new RegisterIoCDependencyPhotonTorpedo().Execute();
        new RegisterIoCDependencyAuthorization().Execute();
        new RegisterIoCDependencyLaunchPhotonTorpedoCommand().Execute();

        var game = new Game();
        Ioc.Register("Game.Current", _ => game);

        var ship = new TestGameObject { Position = new Vector(5, 10) };
        var cmd = Ioc.Resolve<ICommand>("Commands.LaunchPhotonTorpedo", ship, Math.PI / 2);
        cmd.Execute();

        var torpedo = game.GetAll().OfType<PhotonTorpedo>().First();
        Assert.Equal(5, torpedo.Position.X);
        Assert.Equal(10, torpedo.Position.Y);
    }

    [Fact]
    public void ResolveThrowsWhenDependencyNotRegistered()
    {
        Assert.Throws<InvalidOperationException>(() => Ioc.Resolve<object>("Nonexistent"));
    }

    private class TestGameObject : IGameObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Vector Position { get; set; } = new Vector(0, 0);
        public Vector Velocity { get; set; } = new Vector(0, 0);

        public void Update()
        {
            Position = Position + Velocity;
        }
    }
}
