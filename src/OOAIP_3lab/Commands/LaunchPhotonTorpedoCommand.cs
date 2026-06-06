using OOAIP_3lab.Game;

namespace OOAIP_3lab.Commands;

public class LaunchPhotonTorpedoCommand : ICommand
{
    private readonly double _x;
    private readonly double _y;
    private readonly double _direction;
    private readonly string _user;
    private readonly string _token;

    public LaunchPhotonTorpedoCommand(double x, double y, double direction, string user, string token)
    {
        _x = x;
        _y = y;
        _direction = direction;
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    public void Execute()
    {
        var auth = Ioc.Resolve<IAuthorization>("Authorization");
        auth.Authenticate(_user, _token);

        var game = Ioc.Resolve<BaseGame>("Game.Current");
        game.LaunchPhotonTorpedo(_x, _y, _direction);
    }
}