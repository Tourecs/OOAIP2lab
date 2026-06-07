namespace SpaceBattle;

public sealed class AuthCommand : ICommand
{
    private readonly string _subjectId;
    private readonly string _action;
    private readonly string _gameObjectId;

    public AuthCommand(string subjectId, string action, string objectId)
    {
        _subjectId = subjectId;
        _action = action;
        _gameObjectId = objectId;
    }

    public void Execute()
    {
        var isAuthorized = Ioc.Resolve<bool>("Authorization.Check", _subjectId, _action, _gameObjectId);
        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("Игрок не имеет прав совершать действие над этим обьектом");
        }
    }
}