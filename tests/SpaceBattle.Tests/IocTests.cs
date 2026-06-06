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

        var command = Ioc.Resolve<ICommand>(
            "Commands.Send",
            Mock.Of<ICommand>(),
            Mock.Of<ICommandReceiver>()
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
    public void RegisterCommandInjectableDependencyAllowsResolvingByAllRequiredTypes()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();

        _ = Ioc.Resolve<ICommand>("Commands.CommadInjectable");
        _ = Ioc.Resolve<ICommandInjectable>("Commands.CommadInjectable");
        _ = Ioc.Resolve<CommandInjectableCommand>("Commands.CommadInjectable");
    }

    [Fact]
    public void RegisterCommandInjectableDependencyResolvedCommandCanExecuteInjectedCommand()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();

        var innerCommand = new Mock<ICommand>();

        var command = Ioc.Resolve<CommandInjectableCommand>(
            "Commands.CommadInjectable"
        );

        command.Inject(innerCommand.Object);
        command.Execute();

        innerCommand.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void RegisterCommandInjectableDependencyReturnsNewInstanceEachTime()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();

        var first = Ioc.Resolve<CommandInjectableCommand>("Commands.CommadInjectable");
        var second = Ioc.Resolve<CommandInjectableCommand>("Commands.CommadInjectable");

        Assert.NotSame(first, second);
    }

    [Fact]
    public void RegisterActionsStartDependencyAllowsResolvingStartCommand()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var order = new Dictionary<string, object>();

        var command = Ioc.Resolve<ICommand>(
            "Actions.Start",
            order
        );

        Assert.IsType<StartLongOperationCommand>(command);
    }

    [Fact]
    public void ActionsStartSendsInjectableLongOperationToReceiver()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        new RegisterIoCDependencyMacroMoveRotate().Execute();
        new RegisterIoCDependencySendCommand().Execute();
        new RegisterDependencyCommandInjectableCommand().Execute();
        new RegisterIoCDependencyActionsStart().Execute();

        Ioc.Register("Specs.Move", _ => new[] { "Commands.Move" });
        Ioc.Register("Commands.Move", _ => Mock.Of<ICommand>());

        var receiver = new Mock<ICommandReceiver>();

        var order = new Dictionary<string, object>
        {
            ["operation"] = "Move",
            ["object"] = new object(),
            ["receiver"] = receiver.Object
        };

        Ioc.Resolve<ICommand>("Actions.Start", order).Execute();

        Assert.IsType<CommandInjectableCommand>(order["command"]);
        receiver.Verify(x => x.Receive((ICommand)order["command"]), Times.Once);
    }

    [Fact]
    public void ActionsStartCreatesCommandForRotateOperation()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        new RegisterIoCDependencyMacroMoveRotate().Execute();
        new RegisterIoCDependencySendCommand().Execute();
        new RegisterDependencyCommandInjectableCommand().Execute();
        new RegisterIoCDependencyActionsStart().Execute();

        Ioc.Register("Specs.Rotate", _ => new[] { "Commands.Rotate" });
        Ioc.Register("Commands.Rotate", _ => Mock.Of<ICommand>());

        var receiver = new Mock<ICommandReceiver>();

        var order = new Dictionary<string, object>
        {
            ["operation"] = "Rotate",
            ["object"] = new object(),
            ["receiver"] = receiver.Object
        };

        Ioc.Resolve<ICommand>("Actions.Start", order).Execute();

        Assert.IsType<CommandInjectableCommand>(order["command"]);
        receiver.Verify(x => x.Receive((ICommand)order["command"]), Times.Once);
    }

    [Fact]
    public void ActionsStartThrowsWhenOperationIsMissing()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var order = new Dictionary<string, object>
        {
            ["object"] = new object(),
            ["receiver"] = Mock.Of<ICommandReceiver>()
        };

        var command = Ioc.Resolve<ICommand>("Actions.Start", order);

        Assert.Throws<KeyNotFoundException>(() => command.Execute());
    }

    [Fact]
    public void ActionsStartThrowsWhenObjectIsMissing()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var order = new Dictionary<string, object>
        {
            ["operation"] = "Move",
            ["receiver"] = Mock.Of<ICommandReceiver>()
        };

        var command = Ioc.Resolve<ICommand>("Actions.Start", order);

        Assert.Throws<KeyNotFoundException>(() => command.Execute());
    }

    [Fact]
    public void ActionsStartThrowsWhenReceiverIsMissing()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var order = new Dictionary<string, object>
        {
            ["operation"] = "Move",
            ["object"] = new object()
        };

        var command = Ioc.Resolve<ICommand>("Actions.Start", order);

        Assert.Throws<KeyNotFoundException>(() => command.Execute());
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