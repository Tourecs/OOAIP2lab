using System.Collections.Concurrent;
using Xunit;

namespace OOAIP_3lab.Tests;

public class GameRegistryTests
{
    [Fact]
    public void AddObjectToRegistryAddsObject()
    {
        var registry = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry", (object[] args) => registry).Execute();

        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object> { ["Position"] = new Vector(0, 0) };
        var cmd = new AddObjectToRegistryCommand(id, obj);
        cmd.Execute();
        Assert.True(registry.ContainsKey(id));
    }

    [Fact]
    public void AddObjectToRegistryThrowsWhenDuplicate()
    {
        var registry = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry", (object[] args) => registry).Execute();

        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object>();
        registry[id] = obj;
        var cmd = new AddObjectToRegistryCommand(id, obj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void DeleteObjectFromRegistryDeletes()
    {
        var registry = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry", (object[] args) => registry).Execute();

        var id = Guid.NewGuid();
        registry[id] = new Dictionary<string, object>();
        var cmd = new DeleteObjectFromRegistryCommand(id);
        cmd.Execute();
        Assert.False(registry.ContainsKey(id));
    }

    [Fact]
    public void DeleteObjectFromRegistryThrowsWhenNotFound()
    {
        var registry = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry", (object[] args) => registry).Execute();

        var cmd = new DeleteObjectFromRegistryCommand(Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void RegisterIoCDependencyGameRegistryRegistersAll()
    {
        var scope = new ConcurrentDictionary<string, object>();
        Ioc.Resolve<ICommand>("IoC.Register", "Scopes.Current", (object[] args) => (object)scope).Execute();

        new RegisterIoCDependencyGameRegistry().Execute();

        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object> { ["Position"] = new Vector(0, 0) };
        Ioc.Resolve<ICommand>("Game.Registry.Add", id, obj).Execute();
        var retrieved = Ioc.Resolve<IDictionary<string, object>>("Game.Registry.GetObject", id);
        Assert.Same(obj, retrieved);
    }

    [Fact]
    public void RegisterIoCDependencyGameRegistryDeleteAndGetObject()
    {
        var scope = new ConcurrentDictionary<string, object>();
        Ioc.Resolve<ICommand>("IoC.Register", "Scopes.Current", (object[] args) => (object)scope).Execute();

        new RegisterIoCDependencyGameRegistry().Execute();

        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object>();
        Ioc.Resolve<ICommand>("Game.Registry.Add", id, obj).Execute();
        Ioc.Resolve<ICommand>("Game.Registry.Delete", id).Execute();
        Assert.Throws<KeyNotFoundException>(() => Ioc.Resolve<IDictionary<string, object>>("Game.Registry.GetObject", id));
    }
}