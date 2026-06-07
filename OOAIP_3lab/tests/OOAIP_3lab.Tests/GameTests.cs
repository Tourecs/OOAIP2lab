using Moq;
using Xunit;

namespace OOAIP_3lab.Tests;

public class GameTests
{
    private readonly object _scope;
    private readonly Mock<ICommand> _cmd1;
    private readonly Mock<ICommand> _cmd2;
    private readonly Mock<ICommand> _exCmd;
    private readonly Mock<ICommand> _exHandler;
    private readonly Queue<ICommand> _queue;

    public GameTests()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Scopes.Root", (object[] args) => new object()).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Scopes.New", (object[] args) => new object()).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Scopes.Current.Set", (object[] args) =>
            new EmptyCommand()).Execute();

        _scope = Ioc.Resolve<object>("Scopes.New", Ioc.Resolve<object>("Scopes.Root"));
        Ioc.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        _cmd1 = new Mock<ICommand>();
        _cmd2 = new Mock<ICommand>();
        _exCmd = new Mock<ICommand>();
        _exCmd.Setup(c => c.Execute()).Throws<Exception>();
        _exHandler = new Mock<ICommand>();
        _queue = new Queue<ICommand>();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue.Take", (object[] args) => _queue.Dequeue()).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue.Count", (object[] args) => (object)(Func<int>)(() => _queue.Count)).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "ExceptionHandler", (object[] args) => _exHandler.Object).Execute();
    }

    [Fact]
    public void AllCommandsInGameQueueAreExecuted()
    {
        _queue.Enqueue(_cmd1.Object);
        _queue.Enqueue(_cmd2.Object);
        Ioc.Resolve<ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(400)).Execute();

        var game = new Game(_scope);
        game.Execute();

        _cmd1.Verify(c => c.Execute(), Times.Once);
        _cmd2.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void NoCommandsAreExecutedWhenTimeIsUp()
    {
        _queue.Enqueue(_cmd1.Object);
        Ioc.Resolve<ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(-1)).Execute();

        var game = new Game(_scope);
        game.Execute();

        _cmd1.Verify(c => c.Execute(), Times.Never);
    }

    [Fact]
    public void ExceptionHandlerIsExecutedWhenCommandThrows()
    {
        _queue.Enqueue(_exCmd.Object);
        Ioc.Resolve<ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(400)).Execute();

        var game = new Game(_scope);
        game.Execute();

        _exHandler.Verify(h => h.Execute(), Times.Once);
    }
}