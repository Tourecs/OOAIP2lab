using Xunit;

namespace OOAIP_3lab.Tests;

public class AngleTests
{
    public AngleTests()
    {
        Angle.Denominator = 360;
    }

    [Fact]
    public void AngleNormalizesNumerator()
    {
        var a = new Angle(370, 360);
        Assert.Equal(10, a.Numerator);
    }

    [Fact]
    public void AngleImplicitToDouble()
    {
        var a = new Angle(90, 360);
        double d = a;
        Assert.Equal(Math.PI / 2, d, 0.0001);
    }

    [Fact]
    public void AngleAddition()
    {
        var a1 = new Angle(10, 360);
        var a2 = new Angle(20, 360);
        var result = a1 + a2;
        Assert.Equal(30, result.Numerator);
    }

    [Fact]
    public void AngleEquality()
    {
        var a1 = new Angle(90, 360);
        var a2 = new Angle(90, 360);
        Assert.Equal(a1, a2);
    }

    [Fact]
    public void AngleInequality()
    {
        var a1 = new Angle(90, 360);
        var a2 = new Angle(180, 360);
        Assert.NotEqual(a1, a2);
    }

    [Fact]
    public void AngleEqualsNull()
    {
        var a = new Angle(10, 360);
        Assert.False(a.Equals(null!));
    }

    [Fact]
    public void AngleEqualsDifferentType()
    {
        var a = new Angle(10, 360);
        Assert.False(a.Equals("string"));
    }

    [Fact]
    public void AngleBothNullEquality()
    {
        Angle? a = null;
        Angle? b = null;
        Assert.True(a == b);
    }
}