namespace OOAIP_3lab;

public static class AdapterRegistration
{
    public static void Register()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IMovingObject", (object[] args) =>
        {
            var obj = args[0];
            if (obj is not IDictionary<string, object> data)
                throw new ArgumentException("Expected IDictionary<string, object>");
            return new MovingObjectAdapter(data);
        }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IRotatingObject", (object[] args) =>
        {
            var obj = args[0];
            if (obj is not IDictionary<string, object> data)
                throw new ArgumentException("Expected IDictionary<string, object>");
            return new RotatingObjectAdapter(data);
        }).Execute();
    }
}