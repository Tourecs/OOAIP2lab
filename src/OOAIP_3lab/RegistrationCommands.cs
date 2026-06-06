using OOAIP_3lab.Commands;
using OOAIP_3lab.GameObjects;

namespace OOAIP_3lab;

public sealed class RegisterIoCDependencyPhotonTorpedo : ICommand
{
    public void Execute()
    {
        Ioc.Register("GameObjects.PhotonTorpedo", args =>
        {
            var x = (double)args[0];
            var y = (double)args[1];
            var direction = (double)args[2];
            return new PhotonTorpedo(x, y, direction, 5.0);
        });
    }
}

public sealed class RegisterIoCDependencyAuthorization : ICommand
{
    public void Execute()
    {
        Ioc.Register("Authorization", _ => new Authorization());
    }
}

public sealed class RegisterIoCDependencyGame : ICommand
{
    public void Execute()
    {
        Ioc.Register("Game.Current", args =>
        {
            var auth = (IAuthorization)args[0];
            return new Game(auth);
        });
    }
}

public sealed class RegisterIoCDependencyLaunchPhotonTorpedoCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.LaunchPhotonTorpedo", args =>
        {
            var ship = (IGameObject)args[0];
            var direction = (double)args[1];
            var role = (string)args[2];
            var game = (Game)args[3];
            return new LaunchPhotonTorpedoCommand(ship, direction, role, game);
        });
    }
}

public sealed class RegisterIoCDependencyMoveCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Move", args =>
        {
            var gameObject = (IGameObject)args[0];
            return new MoveCommand(gameObject);
        });
    }
}

public sealed class MoveCommand : ICommand
{
    private readonly IGameObject _gameObject;

    public MoveCommand(IGameObject gameObject)
    {
        _gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
    }

    public void Execute()
    {
        _gameObject.Update();
    }
}