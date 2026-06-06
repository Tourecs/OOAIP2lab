namespace OOAIP_3lab.Commands;

public class LaunchPhotonTorpedoCommand : ICommand
{
    private readonly IGameObject _ship;
    private readonly double _direction;

    public LaunchPhotonTorpedoCommand(IGameObject ship, double direction)
    {
        _ship = ship ?? throw new ArgumentNullException(nameof(ship));
        _direction = direction;
    }

    public void Execute()
    {
        var game = Ioc.Resolve<Game>("Game.Current");
        game.LaunchPhotonTorpedo(_ship, _direction);
    }
}
