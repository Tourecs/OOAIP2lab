using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;

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
    public void RegisterMoveDependencyAllowsResolvingMoveCommand()
    {
        Ioc.Register("Adapters.IMovingObject", _ => Mock.Of<IMovingObject>());

        new RegisterIoCDependencyMoveCommand().Execute();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Move",
            new Dictionary<string, object>()
        );

        Assert.IsType<MoveCommand>(command);
    }
}