namespace SpaceBattle;

public class RegisterIoCDependencyGameRegistry : ICommand
{
    public void Execute()
    {
        // Register the Game.Registry factory
        Ioc.Register("Game.Registry", args =>
        {
            var registry = new Dictionary<Guid, IDictionary<string, object>>();
            return registry;
        });

        // Register Game.Registry.Add command factory
        Ioc.Register("Game.Registry.Add", args =>
        {
            var id = (Guid)args[0];
            var obj = (IDictionary<string, object>)args[1];
            return new AddObjectToRegistryCommand(id, obj);
        });

        // Register Game.Registry.Delete command factory
        Ioc.Register("Game.Registry.Delete", args =>
        {
            var id = (Guid)args[0];
            return new DeleteObjectFromRegistryCommand(id);
        });

        // Register Game.Registry.GetObject resolver
        Ioc.Register("Game.Registry.GetObject", args =>
        {
            var id = (Guid)args[0];
            var registry = Ioc.Resolve<Dictionary<Guid, IDictionary<string, object>>>("Game.Registry");
            if (!registry.TryGetValue(id, out var obj))
                throw new KeyNotFoundException($"Object with id {id} not found.");
            return obj;
        });
    }
}
