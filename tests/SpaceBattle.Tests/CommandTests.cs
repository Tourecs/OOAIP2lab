using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;

public sealed class CommandTests
{
    [Fact]
    public void MacroCommandExecutesAllCommands()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        new MacroCommand(first.Object, second.Object).Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void MacroCommandStopsAndThrowsWhenCommandThrows()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        first.Setup(x => x.Execute()).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() =>
            new MacroCommand(first.Object, second.Object).Execute()
        );

        second.Verify(x => x.Execute(), Times.Never);
    }

    [Fact]
    public void MacroCommand_Constructor_Throws_WhenCommandsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MacroCommand(null!)
        );
    }

    [Fact]
    public void SendCommandPassesCommandToReceiver()
    {
        var longCommand = new Mock<ICommand>();
        var receiver = new Mock<ICommandReceiver>();

        new SendCommand(longCommand.Object, receiver.Object).Execute();

        receiver.Verify(x => x.Receive(longCommand.Object), Times.Once);
    }

    [Fact]
    public void SendCommandThrowsWhenReceiverCannotAcceptCommand()
    {
        var receiver = new Mock<ICommandReceiver>();

        receiver
            .Setup(x => x.Receive(It.IsAny<ICommand>()))
            .Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() =>
            new SendCommand(Mock.Of<ICommand>(), receiver.Object).Execute()
        );
    }

    [Fact]
    public void SendCommand_Constructor_Throws_WhenCommandIsNull()
    {
        var receiver = new Mock<ICommandReceiver>();

        Assert.Throws<ArgumentNullException>(() =>
            new SendCommand(null!, receiver.Object)
        );
    }

    [Fact]
    public void SendCommand_Constructor_Throws_WhenReceiverIsNull()
    {
        var innerCommand = new Mock<ICommand>();

        Assert.Throws<ArgumentNullException>(() =>
            new SendCommand(innerCommand.Object, null!)
        );
    }

    [Fact]
    public void CommandInjectableCommand_Execute_ExecutesInjectedCommand()
    {
        var innerCommand = new Mock<ICommand>();
        var command = new CommandInjectableCommand();

        command.Inject(innerCommand.Object);
        command.Execute();

        innerCommand.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void CommandInjectableCommand_Execute_Throws_WhenCommandWasNotInjected()
    {
        var command = new CommandInjectableCommand();

        Assert.Throws<InvalidOperationException>(() =>
            command.Execute()
        );
    }

    [Fact]
    public void CommandInjectableCommand_Inject_Throws_WhenCommandIsNull()
    {
        var command = new CommandInjectableCommand();

        Assert.Throws<ArgumentNullException>(() =>
            command.Inject(null!)
        );
    }

    [Fact]
    public void CommandInjectableCommand_Inject_ReplacesPreviousCommand()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();
        var command = new CommandInjectableCommand();

        command.Inject(first.Object);
        command.Inject(second.Object);
        command.Execute();

        first.Verify(x => x.Execute(), Times.Never);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void MoveCommand_Constructor_Throws_WhenMovingObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MoveCommand(null!)
        );
    }

    [Fact]
    public void RotateCommand_Constructor_Throws_WhenRotatingObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RotateCommand(null!)
        );
    }
}