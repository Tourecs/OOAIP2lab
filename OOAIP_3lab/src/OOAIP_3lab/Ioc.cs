using System.Collections.Concurrent;

namespace OOAIP_3lab;

public static class Ioc
{
    private static readonly ConcurrentDictionary<string, Func<object[], object>> _strategies = new();
    private static readonly ConcurrentDictionary<string, object> _rootScope = new();
    private static ConcurrentDictionary<string, object> _currentScope = _rootScope;

    static Ioc()
    {
        _strategies["IoC.Register"] = args =>
        {
            var key = (string)args[0];
            var strategy = (Func<object[], object>)args[1];
            _strategies[key] = strategy;
            return new RegisterCommand(key, strategy);
        };

        _strategies["Scopes.Root"] = _ => _rootScope;
        _strategies["Scopes.New"] = args =>
        {
            var parent = (ConcurrentDictionary<string, object>)args[0];
            return new ConcurrentDictionary<string, object>(parent);
        };
        _strategies["Scopes.Current"] = _ => _currentScope;
        _strategies["Scopes.Current.Set"] = args =>
        {
            _currentScope = (ConcurrentDictionary<string, object>)args[0];
            return new EmptyCommand();
        };
    }

    public static T Resolve<T>(string key, params object[] args)
    {
        if (_strategies.TryGetValue(key, out var strategy))
        {
            return (T)strategy(args);
        }
        throw new ArgumentException($"Unknown IoC dependency key {key}");
    }

    public static void Clear()
    {
        _strategies.Clear();
        _currentScope = _rootScope;
        _rootScope.Clear();
        _strategies["IoC.Register"] = args =>
        {
            var key = (string)args[0];
            var strategy = (Func<object[], object>)args[1];
            _strategies[key] = strategy;
            return new RegisterCommand(key, strategy);
        };
        _strategies["Scopes.Root"] = _ => _rootScope;
        _strategies["Scopes.New"] = args =>
        {
            var parent = (ConcurrentDictionary<string, object>)args[0];
            return new ConcurrentDictionary<string, object>(parent);
        };
        _strategies["Scopes.Current"] = _ => _currentScope;
        _strategies["Scopes.Current.Set"] = args =>
        {
            _currentScope = (ConcurrentDictionary<string, object>)args[0];
            return new EmptyCommand();
        };
    }

    private class RegisterCommand : ICommand
    {
        private readonly string _key;
        private readonly Func<object[], object> _strategy;
        public RegisterCommand(string key, Func<object[], object> strategy)
        {
            _key = key;
            _strategy = strategy;
        }
        public void Execute()
        {
            _strategies[_key] = _strategy;
        }
    }
}