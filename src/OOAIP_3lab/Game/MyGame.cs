using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab.Game;

public class MyGame : BaseGame
{
    private readonly IGameObjectRepository _repository;

    public MyGame(IGameObjectRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public override void Update()
    {
        foreach (var gameObject in _repository.GetAll().ToList())
        {
            gameObject.Update();
        }
    }

    public override void LaunchPhotonTorpedo(double x, double y, double direction)
    {
        var torpedo = Ioc.Resolve<IGameObject>("GameObjects.PhotonTorpedo", x, y, direction);
        _repository.Add(torpedo);
    }
}