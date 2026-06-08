namespace SpaceBattle;

public static class AdapterRegistration
{
    public static void Register()
    {
        Ioc.Register("Adapters.IMovingObject", (Func<object[], object>)(args =>
        {
            if (args[0] is not IDictionary<string, object> dict)
                throw new ArgumentException("Expected IDictionary<string, object>");
            return new MovingObjectAdapter(dict);
        }));
    }
}
