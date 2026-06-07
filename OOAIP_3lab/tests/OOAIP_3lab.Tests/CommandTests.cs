using Moq;
using Xunit;

namespace OOAIP_3lab.Tests;

public class CommandTests
{
    [Fact]
    public void MacroCommandExecutesAllCommands()
    {
        var executed = 0;
        var cmd1 = new DelegateCommand(() => executed++);
        var cmd2 = new DelegateCommand(() => executed++);
        var macro = new MacroCommand(new ICommand[] { cmd1, cmd2 });
        macro.Execute();
        Assert.Equal(2, executed);
    }

    [Fact]
    public void MacroCommandThrowsWhenEmpty()
    {
        Assert.Throws<ArgumentException>(() => new MacroCommand(Array.Empty<ICommand>()));
    }

    [Fact]
    public void MacroCommandThrowsWhenNull()
    {
        Assert.Throws<ArgumentException>(() => new MacroCommand(null!));
    }

    [Fact]
    public void CommandInjectableThrowsWhenNotInjected()
    {
        var injectable = new CommandInjectableCommand();
        Assert.Throws<InvalidOperationException>(() => injectable.Execute());
    }

    [Fact]
    public void CommandInjectableExecutesInjectedCommand()
    {
        var executed = false;
        var injectable = new CommandInjectableCommand();
        injectable.Inject(new DelegateCommand(() => executed = true));
        injectable.Execute();
        Assert.True(executed);
    }

    [Fact]
    public void EmptyCommandDoesNothing()
    {
        var cmd = new EmptyCommand();
        cmd.Execute();
    }

    [Fact]
    public void SendCommandSendsToReceiver()
    {
        var executed = false;
        var command = new DelegateCommand(() => executed = true);
        var receiver = new Mock<ICommandReceiver>();
        receiver.Setup(r => r.Receive(It.IsAny<ICommand>()))
            .Callback<ICommand>(c => c.Execute());
        var send = new SendCommand(command, receiver.Object);
        send.Execute();
        Assert.True(executed);
    }

    [Fact]
    public void SendCommandThrowsWhenCommandIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SendCommand(null!, new Mock<ICommandReceiver>().Object));
    }

    [Fact]
    public void SendCommandThrowsWhenReceiverIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SendCommand(new EmptyCommand(), null!));
    }

    private class DelegateCommand : ICommand
    {
        private readonly Action _action;
        public DelegateCommand(Action action) => _action = action;
        public void Execute() => _action();
    }
}