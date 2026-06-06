using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;

public sealed class CommandTests
{
    [Fact]
    public void MacroCommandExecutesAllCommandsInOrder()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();
        var sequence = new MockSequence();

        first.InSequence(sequence).Setup(x => x.Execute());
        second.InSequence(sequence).Setup(x => x.Execute());

        var command = new MacroCommand(new[] { first.Object, second.Object });

        command.Execute();

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void MacroCommandThrowsAndStopsWhenCommandThrows()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();

        first.Setup(x => x.Execute()).Throws<InvalidOperationException>();

        var command = new MacroCommand(new[] { first.Object, second.Object });

        Assert.Throws<InvalidOperationException>(() => command.Execute());

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Never);
    }

    [Fact]
    public void MacroCommandConstructorThrowsWhenCommandsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MacroCommand(null!));
    }

    [Fact]
    public void MacroCommandThrowsWhenCommandInListIsNull()
    {
    var command = new MacroCommand(new ICommand[] { null! });

    Assert.Throws<NullReferenceException>(() => command.Execute());
    }

    [Fact]
    public void MacroCommandWithEmptyCommandListDoesNothing()
    {
        var command = new MacroCommand(Array.Empty<ICommand>());

        command.Execute();
    }

    [Fact]
    public void MacroCommandExecutesSingleCommand()
    {
        var inner = new Mock<ICommand>();

        var command = new MacroCommand(new[] { inner.Object });

        command.Execute();

        inner.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]
    public void MacroCommandStopsOnSecondCommandException()
    {
        var first = new Mock<ICommand>();
        var second = new Mock<ICommand>();
        var third = new Mock<ICommand>();

        second.Setup(x => x.Execute()).Throws<InvalidOperationException>();

        var command = new MacroCommand(new[]
        {
            first.Object,
            second.Object,
            third.Object
        });

        Assert.Throws<InvalidOperationException>(() => command.Execute());

        first.Verify(x => x.Execute(), Times.Once);
        second.Verify(x => x.Execute(), Times.Once);
        third.Verify(x => x.Execute(), Times.Never);
    }
}