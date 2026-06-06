namespace SpaceBattle;

public sealed class RegisterIoCDependencyShootCommand : ICommand
{
    public void Execute()
    {
        Ioc.Register("Commands.Shoot", args =>
        {
            var shooterId = (string)args[0];
            var direction = (Vector)args[1];
            var speed = args.Length > 2 ? (int)args[2] : 100;
            var callerId = args.Length > 3 ? (string)args[3] : string.Empty;

            var repository = Ioc.Resolve<SpaceBattle.Repositories.IGameObjectsRepository>("Repositories.GameObjects");
            var authorizer = Ioc.Resolve<SpaceBattle.Security.IAuthorizer>("Security.Authorizer");

            return new SpaceBattle.Commands.ShootCommand(shooterId, direction, speed, repository, authorizer, callerId);
        });
    }
}
