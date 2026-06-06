using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class VectorTests
{
    [Fact]
    public void AdditionReturnsCorrectResult()
    {
        var a = new Vector(1, 2);
        var b = new Vector(3, 4);
        var result = a + b;
        Assert.Equal(new Vector(4, 6), result);
    }

    [Fact]
    public void SubtractionReturnsCorrectResult()
    {
        var a = new Vector(5, 7);
        var b = new Vector(2, 3);
        var result = a - b;
        Assert.Equal(new Vector(3, 4), result);
    }

    [Fact]
    public void SumOfOppositeVectorsReturnsZero()
    {
        Assert.Equal(new Vector(0, 0), new Vector(3, -4) + new Vector(-3, 4));
    }

    [Fact]
    public void FromAngleZeroGivesPositiveX()
    {
        var v = Vector.FromAngle(0, 5);
        Assert.Equal(5, v.X, 10);
        Assert.Equal(0, v.Y, 10);
    }

    [Fact]
    public void FromAngle90GivesPositiveY()
    {
        var v = Vector.FromAngle(Math.PI / 2, 5);
        Assert.Equal(0, v.X, 10);
        Assert.Equal(5, v.Y, 10);
    }

    [Fact]
    public void EqualVectorsAreEqualByEquals()
    {
        Assert.True(new Vector(1, 2).Equals(new Vector(1, 2)));
    }

    [Fact]
    public void EqualVectorsAreEqualByOperator()
    {
        Assert.True(new Vector(1, 2) == new Vector(1, 2));
    }

    [Fact]
    public void DifferentVectorsAreNotEqualByEquals()
    {
        Assert.False(new Vector(1, 2).Equals(new Vector(2, 1)));
    }

    [Fact]
    public void DifferentVectorsAreNotEqualByOperator()
    {
        Assert.True(new Vector(1, 2) != new Vector(2, 1));
    }

    [Fact]
    public void VectorHasHashCode()
    {
        Assert.IsType<int>(new Vector(1, 2).GetHashCode());
    }

    [Fact]
    public void EqualVectorsHaveSameHashCode()
    {
        var a = new Vector(3.14, 2.71);
        var b = new Vector(3.14, 2.71);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToStringReturnsReadableFormat()
    {
        var v = new Vector(1.5, 2.5);
        Assert.Contains("1.5", v.ToString());
        Assert.Contains("2.5", v.ToString());
    }
}
