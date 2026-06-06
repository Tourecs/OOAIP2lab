using OOAIP_3lab.GameObjects;
using Xunit;

namespace OOAIP_3lab.Tests;

public sealed class PhotonTorpedoTests
{
    [Fact]
    public void PhotonTorpedoInitializesWithCorrectPosition()
    {
        var torpedo = new PhotonTorpedo(10, 20, 0, 5.0);
        Assert.Equal(10, torpedo.Position.X);
        Assert.Equal(20, torpedo.Position.Y);
    }

    [Fact]
    public void PhotonTorpedoInitializesWithCorrectDirection()
    {
        var torpedo = new PhotonTorpedo(0, 0, Math.PI / 4, 5.0);
        Assert.Equal(Math.PI / 4, torpedo.Direction);
    }

    [Fact]
    public void PhotonTorpedoVelocityIsSetFromAngle()
    {
        var torpedo = new PhotonTorpedo(0, 0, 0, 5.0);
        Assert.Equal(5.0, torpedo.Velocity.X, 10);
        Assert.Equal(0, torpedo.Velocity.Y, 10);
    }

    [Fact]
    public void PhotonTorpedoUpdatesPositionCorrectly()
    {
        var torpedo = new PhotonTorpedo(0, 0, 0, 5.0);
        torpedo.Update();
        Assert.Equal(5.0, torpedo.Position.X, 10);
        Assert.Equal(0, torpedo.Position.Y, 10);
    }

    [Fact]
    public void PhotonTorpedoMovesDiagonally()
    {
        var torpedo = new PhotonTorpedo(0, 0, Math.PI / 4, Math.Sqrt(2));
        torpedo.Update();
        Assert.Equal(1, torpedo.Position.X, 6);
        Assert.Equal(1, torpedo.Position.Y, 6);
    }

    [Fact]
    public void PhotonTorpedoHasUniqueId()
    {
        var t1 = new PhotonTorpedo(0, 0, 0, 1);
        var t2 = new PhotonTorpedo(0, 0, 0, 1);
        Assert.NotEqual(t1.Id, t2.Id);
    }

    [Fact]
    public void PhotonTorpedoImplementsIGameObject()
    {
        IGameObject torpedo = new PhotonTorpedo(0, 0, 0, 5.0);
        Assert.NotEqual(Guid.Empty, torpedo.Id);
        Assert.Equal(0, torpedo.Position.X);
        Assert.Equal(0, torpedo.Position.Y);
    }

    [Fact]
    public void PhotonTorpedoCannotChangeId()
    {
        var torpedo = new PhotonTorpedo(0, 0, 0, 5.0);
        var idBefore = torpedo.Id;
        torpedo.Update();
        Assert.Equal(idBefore, torpedo.Id);
    }

    [Fact]
    public void PhotonTorpedoPositionChangesAfterUpdate()
    {
        var torpedo = new PhotonTorpedo(1, 2, 0, 3);
        var posBefore = torpedo.Position;
        torpedo.Update();
        Assert.NotEqual(posBefore, torpedo.Position);
    }
}