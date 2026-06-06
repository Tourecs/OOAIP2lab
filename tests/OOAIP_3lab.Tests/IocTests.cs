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
        new RegisterIoCDependencyAuthorization().Execute();
        new RegisterIoCDependencyGame().Execute();
        var auth = Ioc.Resolve<IAuthorization>("Authorization");
        var game = Ioc.Resolve<Game>("Game.Current", auth);
        Assert.IsType<Game>(game);
    }

    [Fact]
    public void RegisterLaunchCommandDependencyAllowsResolving()
    {
        new RegisterIoCDependencyLaunchPhotonTorpedoCommand().Execute();
        var ship = Mock.Of<IGameObject>();
        var game = new Game(new Authorization());
        var cmd = Ioc.Resolve<ICommand>("Commands.LaunchPhotonTorpedo", ship, 0.0, "player", game);
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
    public void MoveCommandCallsUpdateOnGameObject()
    {
        var mockObj = new Mock<IGameObject>();
        var cmd = new MoveCommand(mockObj.Object);
        cmd.Execute();
        mockObj.Verify(o => o.Update(), Times.Once);
    }

    [Fact]
    public void MoveCommandThrowsWhenGameObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MoveCommand(null!));
    }

    [Fact]
    public void FullLaunchFlowWithIoC()
    {
        new RegisterIoCDependencyPhotonTorpedo().Execute();
        new RegisterIoCDependencyAuthorization().Execute();
        new RegisterIoCDependencyLaunchPhotonTorpedoCommand().Execute();

        var auth = new Authorization();
        var game = new Game(auth);

        var ship = Mock.Of<IGameObject>(s => s.Position == new Vector(5, 10));
        var cmd = Ioc.Resolve<ICommand>("Commands.LaunchPhotonTorpedo", ship, Math.PI / 2, "player", game);
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
}