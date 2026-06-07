namespace SpaceBattle;

public class DeleteObjectFromRegistryCommand : ICommand
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
            throw new KeyNotFoundException($"Object with id {_id} not found");
        }
    }
}
