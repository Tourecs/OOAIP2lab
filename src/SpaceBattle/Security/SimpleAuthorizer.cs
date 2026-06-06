namespace SpaceBattle.Security
{
    public sealed class SimpleAuthorizer : IAuthorizer
    {
        public bool CanControl(string callerId, string ownerId)
        {
            if (string.IsNullOrWhiteSpace(callerId) || string.IsNullOrWhiteSpace(ownerId)) return false;
            return callerId == ownerId;
        }
    }
}
