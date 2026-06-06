namespace OOAIP_3lab;

public sealed class Vector : IEquatable<Vector>
{
    public double X { get; set; }
    public double Y { get; set; }

    public Vector(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Vector operator +(Vector a, Vector b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        return new Vector(a.X + b.X, a.Y + b.Y);
    }

    public static Vector operator -(Vector a, Vector b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        return new Vector(a.X - b.X, a.Y - b.Y);
    }

    public static Vector FromAngle(double angle, double magnitude)
    {
        double x = Math.Cos(angle) * magnitude;
        double y = Math.Sin(angle) * magnitude;
        return new Vector(x, y);
    }

    public bool Equals(Vector? other) => other is not null && X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => Equals(obj as Vector);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Vector? left, Vector? right) => Equals(left, right);

    public static bool operator !=(Vector? left, Vector? right) => !Equals(left, right);

    public override string ToString() => $"({X}, {Y})";
}