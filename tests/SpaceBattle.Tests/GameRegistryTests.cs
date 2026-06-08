using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
public sealed class GameRegistryTests : IDisposable
{
    public GameRegistryTests()
    {
        Ioc.Clear();
        new RegisterIoCDependencyGameRegistry().Execute();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void AddAndGetObject_ShouldWork()
    {
        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object> { { "name", "ship" }, { "x", 10 } };

        var addCmd = Ioc.Resolve<ICommand>("Game.Registry.Add", id, obj);
        addCmd.Execute();

        var retrieved = Ioc.Resolve<object>("Game.Registry.GetObject", id);
        Assert.Equal(obj, retrieved);
    }

    [Fact]
    public void GetNonExistent_ShouldThrow()
    {
        var id = Guid.NewGuid();
        Assert.Throws<KeyNotFoundException>(() =>
            Ioc.Resolve<object>("Game.Registry.GetObject", id)
        );
    }

    [Fact]
    public void DeleteNonExistent_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var deleteCmd = Ioc.Resolve<ICommand>("Game.Registry.Delete", id);
        Assert.Throws<KeyNotFoundException>(() => deleteCmd.Execute());
    }

    [Fact]
    public void AddSameIdTwice_ShouldThrowException()
    {
        var id = Guid.NewGuid();
        var obj1 = new Dictionary<string, object> { { "value", 1 } };
        var obj2 = new Dictionary<string, object> { { "value", 2 } };

        var addCmd1 = Ioc.Resolve<ICommand>("Game.Registry.Add", id, obj1);
        addCmd1.Execute();

        var addCmd2 = Ioc.Resolve<ICommand>("Game.Registry.Add", id, obj2);
        Assert.Throws<InvalidOperationException>(() => addCmd2.Execute());
    }

    [Fact]
    public void ObjectCanBeDynamicDictionary_AddFields()
    {
        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object>();
        obj["position"] = new Vector(0, 0);
        obj["velocity"] = new Vector(1, 1);

        var addCmd = Ioc.Resolve<ICommand>("Game.Registry.Add", id, obj);
        addCmd.Execute();

        var retrieved = Ioc.Resolve<IDictionary<string, object>>("Game.Registry.GetObject", id);
        retrieved["newField"] = "added later";

        Assert.Equal("added later", retrieved["newField"]);
    }
}
