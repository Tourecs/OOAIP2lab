using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab;

public class Game : IGameObjectRepository
{
    private readonly List<IGameObject> _gameObjects = new();
    private readonly IAuthorization _auth;

    public Game(IAuthorization auth)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

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

    public void LaunchPhotonTorpedo(IGameObject ship, double direction, string role)
    {
        if (!_auth.CanPerform(role, "LaunchPhotonTorpedo"))
        {
            throw new UnauthorizedAccessException($"Role '{role}' cannot perform LaunchPhotonTorpedo");
        }

        var torpedo = Ioc.Resolve<IGameObject>("GameObjects.PhotonTorpedo", ship.Position.X, ship.Position.Y, direction);
        Add(torpedo);
    }
}