using Xunit;

namespace OOAIP_3lab.Tests;

public class VectorTests
{
    [Fact]
    public void VectorCreatesWithCorrectCoordinates()
    {
        var v = new Vector(1, 2, 3);
        Assert.Equal(new int[] { 1, 2, 3 }, v.Coordinates);
    }

    [Fact]
    public void VectorAddTwoVectors()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(3, 4);
        var result = v1 + v2;
        Assert.Equal(new Vector(4, 6), result);
    }

    [Fact]
    public void VectorAddThrowsOnDifferentDimensions()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(1, 2, 3);
        Assert.Throws<ArgumentException>(() => v1 + v2);
    }

    [Fact]
    public void VectorEquality()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(1, 2);
        Assert.Equal(v1, v2);
        Assert.True(v1 == v2);
    }

    [Fact]
    public void VectorInequality()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(3, 4);
        Assert.NotEqual(v1, v2);
        Assert.True(v1 != v2);
    }

    [Fact]
    public void VectorEqualsNull()
    {
        var v = new Vector(1, 2);
        Assert.False(v.Equals(null));
    }

    [Fact]
    public void VectorEqualsDifferentType()
    {
        var v = new Vector(1, 2);
        Assert.False(v.Equals("string"));
    }

    [Fact]
    public void VectorNullEquality()
    {
        Vector? a = null;
        Vector? b = null;
        Assert.True(a == b);
    }

    [Fact]
    public void VectorHashCode()
    {
        var v1 = new Vector(1, 2);
        var v2 = new Vector(1, 2);
        Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
    }

    [Fact]
    public void VectorThrowsOnEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Vector());
        Assert.Throws<ArgumentException>(() => new Vector(null!));
    }
}