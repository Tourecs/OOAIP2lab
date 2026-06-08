using Xunit;
using System;
using System.Collections.Generic;

namespace SpaceBattle.Tests;

public class AddObjectToRegistryCommandTests
{
    public AddObjectToRegistryCommandTests()
    {
        Ioc.Clear();
    }

    [Fact]
    public void Execute_SuccessfullyAddsObject_WhenIdIsUnique()
    {
        // Ветка 1: Реестр пустой, объект успешно добавляется
        var registry = new Dictionary<Guid, IDictionary<string, object>>();
        Ioc.Register("Game.Registry", (Func<object[], object>)(args => registry));

        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object>();
        var command = new AddObjectToRegistryCommand(id, obj);

        command.Execute();

        Assert.True(registry.ContainsKey(id));
    }

    [Fact]
    public void Execute_ThrowsException_WhenIdAlreadyExists()
    {
        // Ветка 2: ID уже есть в реестре (покрываем ветку throw)
        var id = Guid.NewGuid();
        var registry = new Dictionary<Guid, IDictionary<string, object>>
        {
            [id] = new Dictionary<string, object>()
        };
        Ioc.Register("Game.Registry", (Func<object[], object>)(args => registry));

        var command = new AddObjectToRegistryCommand(id, new Dictionary<string, object>());

        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }
}
