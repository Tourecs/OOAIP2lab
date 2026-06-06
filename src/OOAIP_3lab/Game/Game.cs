using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab.Game;

public class Game : BaseGame, IGameObjectRepository
{
    private readonly List<IGameObject> _gameObjects = new();

    public override void Update()
    {
        foreach (var gameObject in _gameObjects.ToList())
        {
            gameObject.Update();
        }
    }

    public override void LaunchPhotonTorpedo(double x, double y, double direction)
    {
        var torpedo = Ioc.Resolve<IGameObject>("GameObjects.PhotonTorpedo", x, y, direction);
        Add(torpedo);
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
}