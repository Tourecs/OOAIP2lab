using System.Collections;

namespace SpaceBattle;

public sealed class Vector : IEquatable<Vector>, IEnumerable<int>
{
    private readonly int[] _coordinates;

    public Vector(params int[] coordinates)
    {
        ArgumentNullException.ThrowIfNull(coordinates);
        _coordinates = coordinates.ToArray();
    }

    public int Dimensions => _coordinates.Length;

    public int this[int index] => _coordinates[index];

    public static Vector operator +(Vector left, Vector right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (left.Dimensions != right.Dimensions)
        {
            throw new ArgumentException("Vectors must have the same dimensions.");
        }

        return new Vector(left._coordinates.Zip(right._coordinates, (a, b) => a + b).ToArray());
    }

    public bool Equals(Vector? other) => other is not null && _coordinates.SequenceEqual(other._coordinates);

    public override bool Equals(object? obj) => Equals(obj as Vector);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        Array.ForEach(_coordinates, coordinate => hash.Add(coordinate));
        return hash.ToHashCode();
    }

    public static bool operator ==(Vector? left, Vector? right) => Equals(left, right);

    public static bool operator !=(Vector? left, Vector? right) => !Equals(left, right);

    public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)_coordinates).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"({string.Join(", ", _coordinates)})";
}
