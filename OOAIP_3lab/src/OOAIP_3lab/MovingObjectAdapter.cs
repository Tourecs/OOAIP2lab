namespace OOAIP_3lab;

public class MovingObjectAdapter : IMovingObject
{
    private readonly IDictionary<string, object> _data;

    public MovingObjectAdapter(IDictionary<string, object> data)
    {
        _data = data;
    }

    public Vector Position
    {
        get => (Vector)_data["Position"];
        set => _data["Position"] = value;
    }

    public Vector Velocity => (Vector)_data["Velocity"];
}