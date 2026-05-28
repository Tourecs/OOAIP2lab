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
    public void RegisterRotateDependencyAllowsResolvingRotateCommand()
    {
        Ioc.Register("Adapters.IRotatingObject", _ => Mock.Of<IRotatingObject>());

        new RegisterIoCDependencyRotateCommand().Execute();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Rotate",
            new Dictionary<string, object>()
        );

        Assert.IsType<RotateCommand>(command);
    }

    [Fact]
    public void RegisterMacroDependencyAllowsResolvingMacroCommand()
    {
        var first = Mock.Of<ICommand>();
        var second = Mock.Of<ICommand>();

        new RegisterIoCDependencyMacroCommand().Execute();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Macro",
            new[] { first, second }
        );

        Assert.IsType<MacroCommand>(command);
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

    [Fact]
    public void RegisterMacroDependencyAllowsResolvingEmptyMacroCommand()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Macro",
            Array.Empty<ICommand>()
        );

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void RegisterMacroDependencyThrowsNullReferenceWhenCommandsArgumentIsNull()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        Assert.Throws<NullReferenceException>(() =>
            Ioc.Resolve<ICommand>(
                "Commands.Macro",
                (ICommand[])null!
            )
        );
    }

    [Fact]
    public void RegisterThrowsWhenKeyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Ioc.Register(null!, _ => new object())
        );
    }

    [Fact]
    public void RegisterThrowsWhenKeyIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Ioc.Register("", _ => new object())
        );
    }

    [Fact]
    public void RegisterThrowsWhenDependencyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Ioc.Register("Test.Dependency", null!)
        );
    }

    [Fact]
    public void ResolveThrowsWhenKeyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Ioc.Resolve(null!)
        );
    }
	
    [Fact]
    public void RegisterMacroDependencyAllowsResolvingMacroCommandFromSeparateArguments()
    {
        var first = Mock.Of<ICommand>();
        var second = Mock.Of<ICommand>();

        new RegisterIoCDependencyMacroCommand().Execute();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Macro",
            first,
            second
        );

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void ResolveThrowsWhenKeyIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Ioc.Resolve("")
        );
    }

    [Fact]
    public void ResolveThrowsWhenKeyIsWhiteSpace()
    {
        Assert.Throws<ArgumentException>(() =>
            Ioc.Resolve("   ")
        );
    }

    [Fact]
    public void RegisterThrowsWhenKeyIsWhiteSpace()
    {
        Assert.Throws<ArgumentException>(() =>
            Ioc.Register("   ", _ => new object())
        );
    }

    [Fact]
    public void RegisterMacroDependencyThrowsWhenSingleArgumentIsNotCommandArray()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        Assert.Throws<InvalidCastException>(() =>
            Ioc.Resolve<ICommand>(
                "Commands.Macro",
                new object()
            )
        );
    }

    [Fact]
    public void RegisterMacroDependencyThrowsWhenArgumentsContainNonCommand()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        Assert.Throws<InvalidCastException>(() =>
            Ioc.Resolve<ICommand>(
                "Commands.Macro",
                Mock.Of<ICommand>(),
                new object()
            )
        );
    }
}