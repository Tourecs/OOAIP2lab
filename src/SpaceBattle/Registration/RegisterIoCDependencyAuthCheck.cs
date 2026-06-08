namespace SpaceBattle;

public sealed class RegisterIoCDependencyAuthCheck : ICommand
{
    public void Execute()
    {
        Ioc.Register("Authorization.Check",
            (Func<object[], object>)(args =>
            {
                var subjectId = (string)args[0];
                var action = (string)args[1];
                var objectId = (string)args[2];

                var permissions = Ioc.Resolve<Dictionary<string, IEnumerable<string>>>("Authorization.GetPermissions");

                // Check wildcard object
                if (permissions.TryGetValue("*", out var globalActions))
                {
                    if (globalActions.Contains(action))
                        return true;
                }

                // Check specific object
                if (permissions.TryGetValue(objectId, out var objectActions))
                {
                    if (objectActions.Contains("*") || objectActions.Contains(action))
                        return true;
                }

                return false;
            }));
    }
}
