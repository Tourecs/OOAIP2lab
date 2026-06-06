namespace SpaceBattle.Security
{
    public interface IAuthorizer
    {
        bool CanControl(string callerId, string ownerId);
    }
}
