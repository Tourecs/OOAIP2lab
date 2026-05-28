namespace SpaceBattle;

public sealed class Angle : IEquatable<Angle>
{
    public static int Denominator { get; } = 8;

    public Angle(int numerator, int denominator = 8)
    {
        if (denominator != Denominator)
        {
            throw new ArgumentException($"Only denominator {Denominator} is supported.", nameof(denominator));
        }

        Numerator = Normalize(numerator);
    }

    public int Numerator { get; }

    private static int Normalize(int value) => ((value % Denominator) + Denominator) % Denominator;

    public static Angle operator +(Angle left, Angle right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return new Angle(left.Numerator + right.Numerator, Denominator);
    }

    public static implicit operator double(Angle angle)
    {
        ArgumentNullException.ThrowIfNull(angle);
        return 2 * Math.PI * angle.Numerator / Denominator;
    }

    public bool Equals(Angle? other) => other is not null && Numerator == other.Numerator;

    public override bool Equals(object? obj) => Equals(obj as Angle);

    public override int GetHashCode() => Numerator.GetHashCode();

    public static bool operator ==(Angle? left, Angle? right) => Equals(left, right);

    public static bool operator !=(Angle? left, Angle? right) => !Equals(left, right);

    public override string ToString() => $"({Numerator}, {Denominator})";
}
