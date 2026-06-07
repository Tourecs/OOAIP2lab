using SpaceBattle;
using Xunit;
using Moq;

namespace SpaceBattle.Tests;

[Collection("Sequential")]
public class FireCommandTests
{
    public FireCommandTests()
    {
        Ioc.Clear();
        new RegisterIoCDependencyGameRegistry().Execute();
        new RegisterIoCDependencyFireCommand().Execute();
    }

    [Fact]
    public void ReceiveCallsExecuteOnCommand()
    {
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(c => c.Execute()).Verifiable();
        var receiver = new CommandReceiver();

        receiver.Receive(mockCommand.Object);

        mockCommand.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void RegisterIoCDependencyFireCommandRegistersAndCreatesFireCommand()
    {
        var ship = new Dictionary<string, object>();
        var fireCommand = Ioc.Resolve<ICommand>("Commands.Fire", ship);

        Assert.NotNull(fireCommand);
        Assert.IsType<FireCommand>(fireCommand);
    }

    [Fact]
    public void FireCommandThrowsWhenAdaptersNotRegistered()
    {
        var ship = new Dictionary<string, object>();
        var fireCommand = new FireCommand(ship);

        // Should throw because adapters are not registered
        Assert.Throws<InvalidOperationException>(() => fireCommand.Execute());
    }
}
