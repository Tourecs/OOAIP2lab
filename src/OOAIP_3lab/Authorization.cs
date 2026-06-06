using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab;

public class Authorization : IAuthorization
{
    public bool CanPerform(IGameObject obj, string action)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        return action switch
        {
            "LaunchPhotonTorpedo" => true,
            _ => false
        };
    }
}
