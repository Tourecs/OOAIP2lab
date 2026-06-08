using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
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

        Assert.Throws<InvalidOperationException>(() => new MacroCommand(first.Object, second.Object).Execute());
        second.Verify(x => x.Execute(), Times.Never);
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
        receiver.Setup(x => x.Receive(It.IsAny<ICommand>())).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new SendCommand(Mock.Of<ICommand>(), receiver.Object).Execute());
    }

    [Fact]
    public void CommandInjectableExecutesInjectedCommand()
    {
        var command = new Mock<ICommand>();
        var injectable = new CommandInjectableCommand();

        injectable.Inject(command.Object);
        injectable.Execute();

        command.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void CommandInjectableThrowsWhenCommandWasNotInjected()
    {
        Assert.Throws<InvalidOperationException>(() => new CommandInjectableCommand().Execute());
    }

    [Fact]
    public void MacroCommand_Execute_ExecutesAllCommands()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        var command = new MacroCommand(first.Object, second.Object);

        command.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void MacroCommand_Execute_ThrowsAndStops_WhenInnerCommandThrows()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        first
            .Setup(x => x.Execute())
            .Throws(new InvalidOperationException());

        var command = new MacroCommand(first.Object, second.Object);

        Assert.Throws<InvalidOperationException>(() => command.Execute());

        second.Verify(x => x.Execute(), Times.Never);
    }

    [Fact]
    public void MacroCommand_Constructor_Throws_WhenCommandsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MacroCommand(null!));
    }

    [Fact]
    public void SendCommand_Execute_SendsCommandToReceiver()
    {
        var innerCommand = new Mock<ICommand>();
        var receiver = new Mock<ICommandReceiver>();

        var command = new SendCommand(innerCommand.Object, receiver.Object);

        command.Execute();

        receiver.Verify(x => x.Receive(innerCommand.Object), Times.Once);
    }

    [Fact]
    public void SendCommand_Execute_Throws_WhenReceiverThrows()
    {
        var innerCommand = new Mock<ICommand>();
        var receiver = new Mock<ICommandReceiver>();

        receiver
            .Setup(x => x.Receive(It.IsAny<ICommand>()))
            .Throws(new InvalidOperationException());

        var command = new SendCommand(innerCommand.Object, receiver.Object);

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void SendCommand_Constructor_Throws_WhenCommandIsNull()
    {
        var receiver = new Mock<ICommandReceiver>();

        Assert.Throws<ArgumentNullException>(() => new SendCommand(null!, receiver.Object));
    }

    [Fact]
    public void SendCommand_Constructor_Throws_WhenReceiverIsNull()
    {
        var innerCommand = new Mock<ICommand>();

        Assert.Throws<ArgumentNullException>(() => new SendCommand(innerCommand.Object, null!));
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

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    [Fact]
    public void CommandInjectableCommand_Inject_Throws_WhenCommandIsNull()
    {
        var command = new CommandInjectableCommand();

        Assert.Throws<ArgumentNullException>(() => command.Inject(null!));
    }

    [Fact]
    public void EmptyCommand_Execute_DoesNothing()
    {
        var command = new EmptyCommand();

        command.Execute();
    }

    [Fact]
    public void StartLongOperationCommand_Constructor_Throws_WhenOrderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new StartLongOperationCommand(null!));
    }

    [Fact]
    public void StopLongOperationCommand_Constructor_Throws_WhenOrderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new StopLongOperationCommand(null!));
    }
    [Fact]
    public void MoveCommand_Constructor_Throws_WhenMovingObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MoveCommand(null!));
    }

    [Fact]
    public void StartLongOperationCommand_Execute_Throws_WhenOrderHasNoOperation()
    {
        var command = new StartLongOperationCommand(new Dictionary<string, object>());

        Assert.Throws<KeyNotFoundException>(() => command.Execute());
    }

    [Fact]
    public void StopLongOperationCommand_Execute_Throws_WhenOrderHasNoCommand()
    {
        var command = new StopLongOperationCommand(new Dictionary<string, object>());

        Assert.Throws<KeyNotFoundException>(() => command.Execute());
    }

    [Fact]
    public void RotateCommand_Constructor_Throws_WhenRotatingObjectIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RotateCommand(null!));
    }
}
