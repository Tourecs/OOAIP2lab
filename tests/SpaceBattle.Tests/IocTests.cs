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

    [Fact]
    public void ResolveThrowsWhenDependencyIsNotRegistered()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Ioc.Resolve<ICommand>("Unknown.Dependency")
        );
    }

    [Fact]
    public void ResolveThrowsWhenRegisteredFactoryReturnsWrongType()
{
    Ioc.Register("Wrong.Type", _ => new object());

    Assert.Throws<InvalidCastException>(() =>
        Ioc.Resolve<ICommand>("Wrong.Type")
    );
}

    [Fact]
    public void RegisterReplacesExistingDependency()
    {
        Ioc.Register("Test.Dependency", _ => "first");
        Ioc.Register("Test.Dependency", _ => "second");

        var result = Ioc.Resolve<string>("Test.Dependency");

        Assert.Equal("second", result);
    }
}