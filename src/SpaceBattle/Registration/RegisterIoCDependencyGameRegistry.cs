namespace SpaceBattle;

public sealed class RegisterIoCDependencyGameRegistry : ICommand
{
    public void Execute()
    {
        var registry = new Dictionary<Guid, IDictionary<string, object>>();

        Ioc.Register("Game.Registry", (Func<object[], object>)(args => registry));

        Ioc.Register("Game.Registry.Add",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var obj = (IDictionary<string, object>)args[1];
                return new AddObjectToRegistryCommand(id, obj);
            }));

        Ioc.Register("Game.Registry.Delete",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                return new DeleteObjectFromRegistryCommand(id);
            }));

        Ioc.Register("Game.Registry.GetObject",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var reg = Ioc.Resolve<Dictionary<Guid, IDictionary<string, object>>>("Game.Registry");
                if (!reg.TryGetValue(id, out var obj))
                    throw new KeyNotFoundException($"Object with id {id} not found.");
                return obj;
            }));
    }
}