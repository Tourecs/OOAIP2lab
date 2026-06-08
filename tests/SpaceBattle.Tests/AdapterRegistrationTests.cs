using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
public sealed class AdapterRegistrationTests : IDisposable
{
    public AdapterRegistrationTests()
    {
        Ioc.Clear();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void Register_AllowsResolvingIMovingObjectFromDictionary()
    {
        AdapterRegistration.Register();

        var torpedoDict = new Dictionary<string, object>
        {
            ["Position"] = new Vector(1, 2),
            ["Velocity"] = new Vector(10, 0)
        };

        var movingObject = Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", torpedoDict);

        Assert.NotNull(movingObject);
        Assert.Equal(new Vector(1, 2), movingObject.Position);
        Assert.Equal(new Vector(10, 0), movingObject.Velocity);

        movingObject.Position = new Vector(3, 4);
        Assert.Equal(new Vector(3, 4), torpedoDict["Position"]);
    }

    [Fact]
    public void Register_ThrowsWhenArgumentIsNotDictionary()
    {
        AdapterRegistration.Register();

        Assert.Throws<ArgumentException>(() =>
            Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", "not a dictionary"));
    }
}
