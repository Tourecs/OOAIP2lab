namespace OOAIP_3lab;

public class RotatingObjectAdapter : IRotatingObject
{
    private readonly IDictionary<string, object> _data;

    public RotatingObjectAdapter(IDictionary<string, object> data)
    {
        _data = data;
    }

    public Angle? Angle
    {
        get => _data.TryGetValue("Angle", out var val) ? (Angle?)val : null;
        set => _data["Angle"] = value!;
    }

    public Angle? AngularVelocity => _data.TryGetValue("AngularVelocity", out var val) ? (Angle?)val : null;
}