using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab;

public class Game : IGameObjectRepository
{
    private readonly List<IGameObject> _gameObjects = new();

    public void Add(IGameObject gameObject)
    {
        ArgumentNullException.ThrowIfNull(gameObject);
        _gameObjects.Add(gameObject);
    }

    public void Remove(IGameObject gameObject)
    {
        ArgumentNullException.ThrowIfNull(gameObject);
        _gameObjects.Remove(gameObject);
    }

    public IEnumerable<IGameObject> GetAll()
    {
        return _gameObjects;
    }

    public IGameObject GetById(Guid id)
    {
        return _gameObjects.FirstOrDefault(g => g.Id == id)
            ?? throw new InvalidOperationException($"Game object with id '{id}' not found.");
    }

    public void Update()
    {
        foreach (var gameObject in _gameObjects.ToList())
        {
            gameObject.Update();
        }
    }

    public void LaunchPhotonTorpedo(IGameObject ship, double direction)
    {
        var auth = Ioc.Resolve<IAuthorization>("Authorization");
        if (!auth.CanPerform(ship, "LaunchPhotonTorpedo"))
        {
            throw new UnauthorizedAccessException("Object cannot perform LaunchPhotonTorpedo");
        }

        var torpedo = Ioc.Resolve<IGameObject>("GameObjects.PhotonTorpedo", ship.Position.X, ship.Position.Y, direction);
        Add(torpedo);
    }
}
