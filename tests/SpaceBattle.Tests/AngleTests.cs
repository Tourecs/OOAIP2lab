using SpaceBattle;

namespace SpaceBattle.Tests;
[Collection("Sequential")]
public sealed class AngleTests
{
    [Fact]
    public void SumIsNormalizedByDenominator()
    {
        Assert.Equal(new Angle(4, 8), new Angle(5, 8) + new Angle(7, 8));
    }

    [Fact]
    public void EqualAnglesAreEqualByEquals()
    {
        Assert.True(new Angle(15, 8).Equals(new Angle(23, 8)));
    }

    [Fact]
    public void EqualAnglesAreEqualByOperator()
    {
        Assert.True(new Angle(15, 8) == new Angle(23, 8));
    }

    [Fact]
    public void DifferentAnglesAreNotEqualByEquals()
    {
        Assert.False(new Angle(1, 8).Equals(new Angle(2, 8)));
    }

    [Fact]
    public void DifferentAnglesAreNotEqualByOperator()
    {
        Assert.True(new Angle(1, 8) != new Angle(2, 8));
    }

    [Fact]
    public void AngleHasHashCode()
    {
        Assert.IsType<int>(new Angle(1, 8).GetHashCode());
    }

    [Fact]
    public void AngleCanBePassedToMathCosWithoutExplicitCast()
    {
        Assert.Equal(Math.Sqrt(2) / 2, Math.Cos(new Angle(1, 8)), 12);
    }

    [Fact]
    public void Angle_Equals_ReturnsFalse_WhenObjectIsNull()
    {
        var angle = new Angle(1);

        Assert.False(angle.Equals(null));
    }

    [Fact]
    public void Angle_Equals_ReturnsFalse_WhenObjectIsDifferentType()
    {
        var angle = new Angle(1);

        Assert.False(angle.Equals("not angle"));
    }

    [Fact]
    public void Angle_OperatorEquals_ReturnsTrue_WhenSameReference()
    {
        var angle = new Angle(1);

        Assert.True(angle == angle);
    }

    [Fact]
    public void Angle_OperatorEquals_ReturnsFalse_WhenLeftIsNull()
    {
        Angle? left = null;
        var right = new Angle(1);

        Assert.False(left == right);
    }

    [Fact]
    public void Angle_OperatorNotEquals_ReturnsTrue_WhenLeftIsNull()
    {
        Angle? left = null;
        var right = new Angle(1);

        Assert.True(left != right);
    }
}
