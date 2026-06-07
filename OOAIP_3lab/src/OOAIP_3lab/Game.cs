using System.Diagnostics;

namespace OOAIP_3lab;

public class Game : ICommand
{
    private readonly object _scope;
    private readonly Stopwatch _stopwatch;

    public Game(object scope)
    {
        _scope = scope;
        _stopwatch = new Stopwatch();
    }

    public void Execute()
    {
        _stopwatch.Reset();
        Ioc.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

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