using SpaceBattle;

namespace SpaceBattle.Tests;

public sealed class MovingObjectAdapterTests
{
    [Fact]
    public void Constructor_AndProperties_WorkCorrectly()
    {
        var dict = new Dictionary<string, object>
        {
            ["Position"] = new Vector(10, 20),
            ["Velocity"] = new Vector(3, 4)
        };

        var adapter = new MovingObjectAdapter(dict);

        Assert.Equal(new Vector(10, 20), adapter.Position);
        Assert.Equal(new Vector(3, 4), adapter.Velocity);
    }

    [Fact]
    public void Position_Set_UpdatesUnderlyingDictionary()
    {
        var dict = new Dictionary<string, object>
        {
            ["Position"] = new Vector(0, 0),
            ["Velocity"] = new Vector(1, 1)
        };
        var adapter = new MovingObjectAdapter(dict);

        adapter.Position = new Vector(5, 7);

        Assert.Equal(new Vector(5, 7), adapter.Position);
        Assert.Equal(new Vector(5, 7), dict["Position"]);
    }
}