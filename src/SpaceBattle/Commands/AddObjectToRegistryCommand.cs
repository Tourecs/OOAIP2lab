namespace SpaceBattle;

public sealed class AddObjectToRegistryCommand : ICommand
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
            throw new InvalidOperationException($"Объект с id {_id} уже существует");
        }

        registry[_id] = _object;
    }
}