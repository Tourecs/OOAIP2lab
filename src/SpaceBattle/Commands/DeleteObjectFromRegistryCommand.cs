namespace SpaceBattle;

public sealed class DeleteObjectFromRegistryCommand : ICommand
{
    private readonly Guid _id;

    public DeleteObjectFromRegistryCommand(Guid id)
    {
        _id = id;
    }

    public void Execute()
    {
        var registry = Ioc.Resolve<Dictionary<Guid, IDictionary<string, object>>>("Game.Registry");
        if (!registry.Remove(_id))
        {
            throw new KeyNotFoundException($"Объект с id {_id} не найден");
        }
    }
}