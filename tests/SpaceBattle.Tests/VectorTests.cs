using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
public sealed class VectorTests
{
    [Fact]
    public void SumOfOppositeVectorsReturnsZeroVector()
    {
        Assert.Equal(new Vector(0, 0, 0), new Vector(1, -1, 2) + new Vector(-1, 1, -2));
    }

    [Fact]
    public void SumThrowsWhenLeftVectorHasMoreDimensions()
    {
        Assert.Throws<ArgumentException>(() => new Vector(1, 2, 3) + new Vector(1, 2));
    }

    [Fact]
    public void SumThrowsWhenRightVectorHasMoreDimensions()
    {
        Assert.Throws<ArgumentException>(() => new Vector(1, 2) + new Vector(1, 2, 3));
    }

    [Fact]
    public void EqualCoordinateVectorsAreEqualByEquals()
    {
        Assert.True(new Vector(1, 2).Equals(new Vector(1, 2)));
    }

    [Fact]
    public void EqualCoordinateVectorsAreEqualByOperator()
    {
        Assert.True(new Vector(1, 2) == new Vector(1, 2));
    }

    [Fact]
    public void DifferentCoordinateVectorsAreNotEqualByEquals()
    {
        Assert.False(new Vector(1, 2).Equals(new Vector(2, 1)));
    }

    [Fact]
    public void DifferentCoordinateVectorsAreNotEqualByOperator()
    {
        Assert.True(new Vector(1, 2) != new Vector(2, 1));
    }

    [Fact]
    public void VectorHasHashCode()
    {
        Assert.IsType<int>(new Vector(1, 2).GetHashCode());
    }

    [Fact]
    public void Vector_Equals_ReturnsFalse_WhenObjectIsNull()
    {
        var vector = new Vector(1, 2);

        Assert.False(vector.Equals(null));
    }

    [Fact]
    public void Vector_Equals_ReturnsFalse_WhenObjectIsDifferentType()
    {
        var vector = new Vector(1, 2);

        Assert.False(vector.Equals("not vector"));
    }

    [Fact]
    public void Vector_OperatorEquals_ReturnsTrue_WhenSameReference()
    {
        var vector = new Vector(1, 2);

        Assert.True(vector == vector);
    }

    [Fact]
    public void Vector_OperatorEquals_ReturnsFalse_WhenLeftIsNull()
    {
        Vector? left = null;
        var right = new Vector(1, 2);

        Assert.False(left == right);
    }

    [Fact]
    public void Vector_OperatorNotEquals_ReturnsTrue_WhenLeftIsNull()
    {
        Vector? left = null;
        var right = new Vector(1, 2);

        Assert.True(left != right);
    }
}
