using SpaceBattle;

namespace SpaceBattle.Tests;

public sealed class GameTests : IDisposable
{
    private readonly Mock<ICommand> _cmd1;
    private readonly Mock<ICommand> _cmd2;
    private readonly Mock<ICommand> _exCmd;
    private readonly Mock<ICommand> _exHandler;
    private readonly Queue<ICommand> _queue;

    public GameTests()
    {
        Ioc.Clear();

        _cmd1 = new Mock<ICommand>();
        _cmd2 = new Mock<ICommand>();
        _exCmd = new Mock<ICommand>();
        _exCmd.Setup(c => c.Execute()).Throws<System.Exception>();
        _exHandler = new Mock<ICommand>();
        _queue = new Queue<ICommand>();

        Ioc.Register("Game.Queue.Take", (Func<object[], object>)(args => _queue.Dequeue()));
        Ioc.Register("Game.Queue.Count", (Func<object[], object>)(args =>
        {
            Func<int> countFunc = () => _queue.Count;
            return (object)countFunc;
        }));
        Ioc.Register("ExceptionHandler", (Func<object[], object>)(args => _exHandler.Object));
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void AllCommandsInGameQueueAreExecuted()
    {
        _queue.Enqueue(_cmd1.Object);
        _queue.Enqueue(_cmd2.Object);
        Ioc.Register("Command.Time", (Func<object[], object>)(args => (object)TimeSpan.FromMilliseconds(400)));

        var game = new Game();
        game.Execute();

        _cmd1.Verify(c => c.Execute(), Times.Once);
        _cmd2.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void NoCommandsAreExecutedWhenTimeIsUp()
    {
        _queue.Enqueue(_cmd1.Object);
        Ioc.Register("Command.Time", (Func<object[], object>)(args => (object)TimeSpan.FromMilliseconds(-1)));

        var game = new Game();
        game.Execute();

        _cmd1.Verify(c => c.Execute(), Times.Never);
    }

    [Fact]
    public void ExceptionHandlerIsExecutedWhenCommandThrows()
    {
        _queue.Enqueue(_exCmd.Object);
        Ioc.Register("Command.Time", (Func<object[], object>)(args => (object)TimeSpan.FromMilliseconds(400)));

        var game = new Game();
        game.Execute();

        _exHandler.Verify(h => h.Execute(), Times.Once);
    }
}