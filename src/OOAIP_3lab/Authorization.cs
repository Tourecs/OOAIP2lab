namespace OOAIP_3lab;

public interface IAuthorization
{
    bool IsAuthorized(string user, string token);
    void Authenticate(string user, string token);
}

public class Authorization : IAuthorization
{
    public bool IsAuthorized(string user, string token)
    {
        return !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(token);
    }

    public void Authenticate(string user, string token)
    {
        if (!IsAuthorized(user, token))
        {
            throw new UnauthorizedAccessException("Authentication failed");
        }
    }
}