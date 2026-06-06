using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab.Game;

public abstract class BaseGame
{
    public abstract void Update();
    public abstract void LaunchPhotonTorpedo(double x, double y, double direction);
}