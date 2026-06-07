using System.Diagnostics;

namespace SpaceBattle;

public sealed class Game : ICommand
{
    private readonly Stopwatch _stopwatch;

    public Game()
    {
        _stopwatch = new Stopwatch();
    }

    public void Execute()
    {
        _stopwatch.Reset();

        var commandsTime = Ioc.Resolve<TimeSpan>("Command.Time");

        while (Ioc.Resolve<Func<int>>("Game.Queue.Count")() > 0 && _stopwatch.Elapsed <= commandsTime)
        {
            _stopwatch.Start();
            var cmd = Ioc.Resolve<ICommand>("Game.Queue.Take");
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Ioc.Resolve<ICommand>("ExceptionHandler", ex, cmd).Execute();
            }
            _stopwatch.Stop();
        }
    }
}