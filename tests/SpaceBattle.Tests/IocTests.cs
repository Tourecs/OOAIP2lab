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
        new RegisterIoCDependencyMacroCommand().Execute();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Macro",
            Array.Empty<ICommand>()
        );

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void RegisterMacroMoveRotateAllowsResolvingMoveMacro()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        new RegisterIoCDependencyMacroMoveRotate().Execute();

        Ioc.Register("Specs.Move", _ => new[] { "Commands.Move" });
        Ioc.Register("Commands.Move", _ => Mock.Of<ICommand>());

        var command = Ioc.Resolve<ICommand>(
            "Macro.Move",
            new object()
        );

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void RegisterMacroMoveRotateAllowsResolvingRotateMacro()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        new RegisterIoCDependencyMacroMoveRotate().Execute();

        Ioc.Register("Specs.Rotate", _ => new[] { "Commands.Rotate" });
        Ioc.Register("Commands.Rotate", _ => Mock.Of<ICommand>());

        var command = Ioc.Resolve<ICommand>(
            "Macro.Rotate",
            new object()
        );

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void RegisterSendDependencyAllowsResolvingSendCommand()
    {
        new RegisterIoCDependencySendCommand().Execute();

        var innerCommand = Mock.Of<ICommand>();
        var receiver = Mock.Of<ICommandReceiver>();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Send",
            innerCommand,
            receiver
        );

        Assert.IsType<SendCommand>(command);
    }

    [Fact]
    public void RegisterSendDependencyResolvedCommandSendsInnerCommandToReceiver()
    {
        new RegisterIoCDependencySendCommand().Execute();

        var innerCommand = Mock.Of<ICommand>();
        var receiver = new Mock<ICommandReceiver>();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Send",
            innerCommand,
            receiver.Object
        );

        command.Execute();

        receiver.Verify(x => x.Receive(innerCommand), Times.Once);
    }

    [Fact]
    public void RegisterSendDependencyThrowsWhenCommandArgumentIsMissing()
    {
        new RegisterIoCDependencySendCommand().Execute();

        Assert.Throws<IndexOutOfRangeException>(() =>
            Ioc.Resolve<ICommand>(
                "Commands.Send",
                Mock.Of<ICommand>()
            )
        );
    }

    [Fact]
    public void RegisterSendDependencyThrowsWhenFirstArgumentHasWrongType()
    {
        new RegisterIoCDependencySendCommand().Execute();

        Assert.Throws<InvalidCastException>(() =>
            Ioc.Resolve<ICommand>(
                "Commands.Send",
                new object(),
                Mock.Of<ICommandReceiver>()
            )
        );
    }

    [Fact]
    public void RegisterSendDependencyThrowsWhenSecondArgumentHasWrongType()
    {
        new RegisterIoCDependencySendCommand().Execute();

        Assert.Throws<InvalidCastException>(() =>
            Ioc.Resolve<ICommand>(
                "Commands.Send",
                Mock.Of<ICommand>(),
                new object()
            )
        );
    }

    [Fact]
    public void MacroStrategyResolvesMacroAndExecutesCommands()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        new RegisterIoCDependencyMacroCommand().Execute();

        Ioc.Register("Specs.Test", _ => new[] { "Commands.First", "Commands.Second" });
        Ioc.Register("Commands.First", _ => first.Object);
        Ioc.Register("Commands.Second", _ => second.Object);

        var command = new CreateMacroCommandStrategy("Test").Resolve(Array.Empty<object>());

        command.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void MacroStrategyThrowsWhenSpecDoesNotExist()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new CreateMacroCommandStrategy("Unknown").Resolve(Array.Empty<object>())
        );
    }

    [Fact]
    public void MacroStrategyThrowsWhenCommandDoesNotExist()
    {
        Ioc.Register("Specs.Test", _ => new[] { "Commands.Missing" });

        Assert.Throws<InvalidOperationException>(() =>
            new CreateMacroCommandStrategy("Test").Resolve(Array.Empty<object>())
        );
    }

    [Fact]
    public void IocResolveThrowsWhenDependencyIsNotRegistered()
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
    public void RegisterThrowsWhenKeyIsWhiteSpace()
    {
        Assert.Throws<ArgumentException>(() =>
            Ioc.Register("   ", _ => new object())
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

    [Fact]
    public void CreateMacroCommandStrategyConstructorThrowsWhenSpecIsNull()
    {
        Assert.Throws<ArgumentException>(() =>
            new CreateMacroCommandStrategy(null!)
        );
    }

    [Fact]
    public void CreateMacroCommandStrategyConstructorThrowsWhenSpecIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new CreateMacroCommandStrategy("")
        );
    }

    [Fact]
    public void CreateMacroCommandStrategyConstructorThrowsWhenSpecIsWhiteSpace()
    {
        Assert.Throws<ArgumentException>(() =>
            new CreateMacroCommandStrategy("   ")
        );
    }

    [Fact]
    public void MacroStrategyThrowsWhenArgsIsNull()
    {
        var strategy = new CreateMacroCommandStrategy("Test");

        Assert.Throws<ArgumentNullException>(() =>
            strategy.Resolve(null!)
        );
    }
}