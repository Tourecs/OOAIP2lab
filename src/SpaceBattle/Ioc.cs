using System.Collections.Concurrent;

namespace SpaceBattle;

public static class Ioc
{
    private static readonly ConcurrentDictionary<string, Func<object[], object>> Dependencies = new();

    public static void Register(string key, Func<object[], object> dependency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(dependency);
        Dependencies[key] = dependency;
    }

    public static object Resolve(string key, params object[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!Dependencies.TryGetValue(key, out var dependency))
        {
            throw new InvalidOperationException($"Dependency '{key}' is not registered.");
        }

        return dependency(args);
    }

    public static T Resolve<T>(string key, params object[] args) => (T)Resolve(key, args);

    public static void Clear() => Dependencies.Clear();
}
