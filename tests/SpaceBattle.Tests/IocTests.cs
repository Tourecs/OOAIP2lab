using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
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

        var command = Ioc.Resolve<ICommand>("Commands.Move", new Dictionary<string, object>());

        Assert.IsType<MoveCommand>(command);
    }

    [Fact]
    public void RegisterRotateDependencyAllowsResolvingRotateCommand()
    {
        Ioc.Register("Adapters.IRotatingObject", _ => Mock.Of<IRotatingObject>());
        new RegisterIoCDependencyRotateCommand().Execute();

        var command = Ioc.Resolve<ICommand>("Commands.Rotate", new Dictionary<string, object>());

        Assert.IsType<RotateCommand>(command);
    }

    [Fact]
    public void RegisterMacroDependencyAllowsResolvingMacroCommand()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        var command = Ioc.Resolve<ICommand>("Commands.Macro", Array.Empty<ICommand>());

        Assert.IsType<MacroCommand>(command);
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
        Assert.Throws<InvalidOperationException>(() => new CreateMacroCommandStrategy("Unknown").Resolve(Array.Empty<object>()));
    }

    [Fact]
    public void MacroStrategyThrowsWhenCommandDoesNotExist()
    {
        Ioc.Register("Specs.Test", _ => new[] { "Commands.Missing" });

        Assert.Throws<InvalidOperationException>(() => new CreateMacroCommandStrategy("Test").Resolve(Array.Empty<object>()));
    }

    [Fact]
    public void RegisterMacroMoveRotateAllowsResolvingMoveMacro()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        new RegisterIoCDependencyMacroMoveRotate().Execute();
        Ioc.Register("Specs.Move", _ => new[] { "Commands.Move" });
        Ioc.Register("Commands.Move", _ => Mock.Of<ICommand>());

        var command = Ioc.Resolve<ICommand>("Macro.Move", new object());

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void RegisterMacroMoveRotateAllowsResolvingRotateMacro()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        new RegisterIoCDependencyMacroMoveRotate().Execute();
        Ioc.Register("Specs.Rotate", _ => new[] { "Commands.Rotate" });
        Ioc.Register("Commands.Rotate", _ => Mock.Of<ICommand>());

        var command = Ioc.Resolve<ICommand>("Macro.Rotate", new object());

        Assert.IsType<MacroCommand>(command);
    }

    [Fact]
    public void RegisterSendDependencyAllowsResolvingSendCommand()
    {
        new RegisterIoCDependencySendCommand().Execute();

        var command = Ioc.Resolve<ICommand>("Commands.Send", Mock.Of<ICommand>(), Mock.Of<ICommandReceiver>());

        Assert.IsType<SendCommand>(command);
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
    public void RegisterActionsStartDependencyAllowsResolvingStartCommand()
    {
        new RegisterIoCDependencyActionsStart().Execute();
        var order = new Dictionary<string, object>();

        var command = Ioc.Resolve<ICommand>("Actions.Start", order);

        Assert.IsType<StartLongOperationCommand>(command);
    }

    [Fact]
    public void RegisterActionsStopDependencyAllowsResolvingStopCommand()
    {
        new RegisterIoCDependencyActionsStop().Execute();
        var order = new Dictionary<string, object>();

        var command = Ioc.Resolve<ICommand>("Actions.Stop", order);

        Assert.IsType<StopLongOperationCommand>(command);
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
    public void ActionsStopInjectsEmptyCommandInConstantTimeWithoutReceiverOrQueueLookup()
    {
        new RegisterIoCDependencyActionsStop().Execute();
        var inner = new Mock<ICommand>();
        var injectable = new CommandInjectableCommand();
        injectable.Inject(inner.Object);
        var order = new Dictionary<string, object> { ["command"] = injectable };

        Ioc.Resolve<ICommand>("Actions.Stop", order).Execute();
        injectable.Execute();

        inner.Verify(x => x.Execute(), Times.Never);
    }

    [Fact]
    public void Ioc_Resolve_Throws_WhenDependencyIsNotRegistered()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Ioc.Resolve<ICommand>("Unknown.Dependency"));
    }
    [Fact]
    public void RegisterIoCDependencyMacroCommand_Execute_RegistersDependency_FromParams()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        var command = Ioc.Resolve<ICommand>(
            "Commands.Macro",
            first.Object,
            second.Object
        );

        command.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void RegisterIoCDependencyMacroCommand_Execute_RegistersDependency_FromArray()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        var commands = new ICommand[]
        {
            first.Object,
            second.Object
        };

        var command = Ioc.Resolve<ICommand>(
            "Commands.Macro",
            commands
        );

        command.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void RegisterIoCDependencyActionsStart_Execute_RegistersDependency()
    {
        Ioc.Register("Macro.Move", _ => new EmptyCommand());
        Ioc.Register("Commands.CommandInjectable", _ => new CommandInjectableCommand());
        Ioc.Register("Commands.Send", args => new SendCommand(
            (ICommand)args[0],
            (ICommandReceiver)args[1]
        ));

        new RegisterIoCDependencyActionsStart().Execute();

        var receiver = new Mock<ICommandReceiver>();

        var order = new Dictionary<string, object>
        {
            ["operation"] = "Move",
            ["object"] = new object(),
            ["receiver"] = receiver.Object
        };

        var command = Ioc.Resolve<ICommand>("Actions.Start", order);

        command.Execute();

        receiver.Verify(x => x.Receive(It.IsAny<ICommand>()), Times.Once);
        Assert.True(order.ContainsKey("command"));
    }

    [Fact]
    public void RegisterIoCDependencyActionsStop_Execute_RegistersDependency()
    {
        new RegisterIoCDependencyActionsStop().Execute();

        var injectedInnerCommand = new Mock<ICommand>();
        var injectable = new CommandInjectableCommand();
        injectable.Inject(injectedInnerCommand.Object);

        var order = new Dictionary<string, object>
        {
            ["command"] = injectable
        };

        var command = Ioc.Resolve<ICommand>("Actions.Stop", order);

        command.Execute();
        injectable.Execute();

        injectedInnerCommand.Verify(x => x.Execute(), Times.Never);
    }
    [Fact]
    public void RegisterIoCDependencyMoveCommand_Resolve_ReturnsMoveCommand()
    {
        var gameObject = new object();
        var movingObject = new Mock<IMovingObject>();

        Ioc.Register("Adapters.IMovingObject", args => movingObject.Object);

        new RegisterIoCDependencyMoveCommand().Execute();

        var command = Ioc.Resolve<ICommand>("Commands.Move", gameObject);

        Assert.IsType<MoveCommand>(command);
    }

    [Fact]
    public void RegisterIoCDependencyRotateCommand_Resolve_ReturnsRotateCommand()
    {
        var gameObject = new object();
        var rotatingObject = new Mock<IRotatingObject>();

        Ioc.Register("Adapters.IRotatingObject", args => rotatingObject.Object);

        new RegisterIoCDependencyRotateCommand().Execute();

        var command = Ioc.Resolve<ICommand>("Commands.Rotate", gameObject);

        Assert.IsType<RotateCommand>(command);
    }

    [Fact]
    public void RegisterIoCDependencyActionsStop_Execute_ReplacesCommandWithEmptyCommand()
    {
        new RegisterIoCDependencyActionsStop().Execute();

        var inner = new Mock<ICommand>();
        var injectable = new CommandInjectableCommand();
        injectable.Inject(inner.Object);

        var order = new Dictionary<string, object>
        {
            ["command"] = injectable
        };

        var stop = Ioc.Resolve<ICommand>("Actions.Stop", order);

        stop.Execute();
        injectable.Execute();

        inner.Verify(x => x.Execute(), Times.Never);
    }
    [Fact]
    public void RegisterIoCDependencyMacroCommand_Resolve_FromArray()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        var commands = new ICommand[] { first.Object, second.Object };

        var macro = Ioc.Resolve<ICommand>("Commands.Macro", commands);

        macro.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void RegisterIoCDependencyMacroCommand_Resolve_FromParams()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        var macro = Ioc.Resolve<ICommand>("Commands.Macro", first.Object, second.Object);

        macro.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }
}
