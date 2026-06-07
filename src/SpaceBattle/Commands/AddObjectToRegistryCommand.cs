namespace SpaceBattle;

public class AddObjectToRegistryCommand : ICommand
{
    private readonly Guid _id;
    private readonly IDictionary<string, object> _object;

    public AddObjectToRegistryCommand(Guid id, IDictionary<string, object> obj)
    {
        _id = id;
        _object = obj;
    }

    public void Execute()
    {
        var registry = Ioc.Resolve<Dictionary<Guid, IDictionary<string, object>>>("Game.Registry");

        if (registry.ContainsKey(_id))
        {
            throw new InvalidOperationException($"Object with id {_id} already exists");
        }

        registry[_id] = _object;
    }
}
