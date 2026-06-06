namespace OOAIP_3lab.Commands;

public class LaunchPhotonTorpedoCommand : ICommand
{
    private readonly IGameObject _ship;
    private readonly double _direction;
    private readonly string _role;
    private readonly Game _game;

    public LaunchPhotonTorpedoCommand(IGameObject ship, double direction, string role, Game game)
    {
        _ship = ship ?? throw new ArgumentNullException(nameof(ship));
        _direction = direction;
        _role = role ?? throw new ArgumentNullException(nameof(role));
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public void Execute()
    {
        _game.LaunchPhotonTorpedo(_ship, _direction, _role);
    }
}