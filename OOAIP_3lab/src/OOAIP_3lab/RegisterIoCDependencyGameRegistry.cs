namespace OOAIP_3lab;

public class RegisterIoCDependencyGameRegistry : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry",
            (Func<object[], object>)(args =>
            {
                var currentScope = (IDictionary<string, object>)Ioc.Resolve<object>("Scopes.Current");
                if (!currentScope.ContainsKey("GameRegistry"))
                    currentScope["GameRegistry"] = new Dictionary<Guid, IDictionary<string, object>>();
                return currentScope["GameRegistry"];
            })
        ).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry.Add",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var obj = (IDictionary<string, object>)args[1];
                return new AddObjectToRegistryCommand(id, obj);
            })
        ).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry.Delete",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                return new DeleteObjectFromRegistryCommand(id);
            })
        ).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Game.Registry.GetObject",
            (Func<object[], object>)(args =>
            {
                var id = (Guid)args[0];
                var registry = Ioc.Resolve<Dictionary<Guid, IDictionary<string, object>>>("Game.Registry");
                if (!registry.TryGetValue(id, out var obj))
                    throw new KeyNotFoundException($"Object with id {id} not found.");
                return obj;
            })
        ).Execute();
    }
}