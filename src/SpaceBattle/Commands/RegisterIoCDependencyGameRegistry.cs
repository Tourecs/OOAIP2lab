namespace SpaceBattle;

public class RegisterIoCDependencyGameRegistry : ICommand
{
    private static Dictionary<Guid, IDictionary<string, object>> _gameRegistry = new();

    public void Execute()
    {
        // Initialize the shared registry
        _gameRegistry = new Dictionary<Guid, IDictionary<string, object>>();

        // Register the Game.Registry factory that returns the same instance
        Ioc.Register("Game.Registry", args =>
        {
            return _gameRegistry;
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
