using System.Collections.Concurrent;
using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab;

public interface IAuthorization
{
    bool CanPerform(string role, string action);
}

public class Authorization : IAuthorization
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _permissions = new();

    public Authorization()
    {
        Grant("admin", "LaunchPhotonTorpedo");
        Grant("player", "LaunchPhotonTorpedo");
        Grant("player", "Move");
        Grant("observer", "View");
    }

    public bool CanPerform(string role, string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        return _permissions.TryGetValue(role, out var actions) && actions.Contains(action);
    }

    public void Grant(string role, string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var actions = _permissions.GetOrAdd(role, _ => new HashSet<string>());
        lock (actions)
        {
            actions.Add(action);
        }
    }

    public void Revoke(string role, string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        if (_permissions.TryGetValue(role, out var actions))
        {
            lock (actions)
            {
                actions.Remove(action);
            }
        }
    }
}