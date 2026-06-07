using Moq;
using Xunit;

namespace OOAIP_3lab.Tests;

public class StartStopCommandTests
{
    [Fact]
    public void StartCommandThrowsWhenNoReceiver()
    {
        var obj = new Dictionary<string, object>();
        Assert.Throws<InvalidOperationException>(() => new StartCommand(obj, "Move"));
    }

    [Fact]
    public void StopCommandThrowsWhenOperationNotStarted()
    {
        var obj = new Dictionary<string, object>();
        var cmd = new StopCommand(obj, "Move");
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void StopCommandInjectsEmptyCommandAndRemovesKey()
    {
        Angle.Denominator = 360;
        var injectable = new CommandInjectableCommand();
        var obj = new Dictionary<string, object>
        {
            ["repeatableMove"] = injectable,
            ["Receiver"] = new Mock<ICommandReceiver>().Object
        };

        var cmd = new StopCommand(obj, "Move");
        cmd.Execute();
        Assert.False(obj.ContainsKey("repeatableMove"));
    }

    [Fact]
    public void RegisterIoCDependencyActionsStartRegisters()
    {
        new RegisterIoCDependencyActionsStart().Execute();
        var obj = new Dictionary<string, object> { ["Receiver"] = new Mock<ICommandReceiver>().Object };
        var cmd = Ioc.Resolve<ICommand>("Actions.Start", obj, "Move");
        Assert.IsType<StartCommand>(cmd);
    }

    [Fact]
    public void RegisterIoCDependencyActionsStopRegisters()
    {
        new RegisterIoCDependencyActionsStop().Execute();
        var obj = new Dictionary<string, object>();
        var cmd = Ioc.Resolve<ICommand>("Actions.Stop", obj, "Move");
        Assert.IsType<StopCommand>(cmd);
    }

    [Fact]
    public void RegisterIoCDependencyCommandInjectableCreatesNew()
    {
        new RegisterIoCDependencyCommandInjectableCommand().Execute();
        var cmd1 = Ioc.Resolve<ICommand>("Commands.CommandInjectable");
        var cmd2 = Ioc.Resolve<ICommand>("Commands.CommandInjectable");
        Assert.NotSame(cmd1, cmd2);
    }

    [Fact]
    public void CeateMacroCommandStrategyCreatesMacro()
    {
        var cmd1 = new EmptyCommand();
        var cmd2 = new EmptyCommand();
        var macro = CeateMacroCommandStrategy.Create(cmd1, cmd2);
        Assert.IsType<MacroCommand>(macro);
    }

    [Fact]
    public void RegisterIoCDependencyMacroMoveRotateRegistersBoth()
    {
        new RegisterIoCDependencyMacroMoveRotate().Execute();
        var cmd = new EmptyCommand();
        var send = new SendCommand(new EmptyCommand(), new Mock<ICommandReceiver>().Object);
        var moveMacro = Ioc.Resolve<ICommand>("Macro.Move", cmd, send);
        var rotateMacro = Ioc.Resolve<ICommand>("Macro.Rotate", cmd, send);
        Assert.IsType<MacroCommand>(moveMacro);
        Assert.IsType<MacroCommand>(rotateMacro);
    }

    [Fact]
    public void RegisterIoCDependencySendCommandRegisters()
    {
        new RegisterIoCDependencySendCommand().Execute();
        var cmd = new EmptyCommand();
        var receiver = new Mock<ICommandReceiver>().Object;
        var send = Ioc.Resolve<ICommand>("Commands.Send", cmd, receiver);
        Assert.IsType<SendCommand>(send);
    }
}