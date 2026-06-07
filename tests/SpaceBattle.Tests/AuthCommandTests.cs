using SpaceBattle;

namespace SpaceBattle.Tests;

public sealed class AuthCommandTests : IDisposable
{
    public AuthCommandTests()
    {
        Ioc.Clear();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    [Fact]
    public void AuthCommand_Successfully_Authorizes()
    {
        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        Ioc.Register("Authorization.Check",
            (Func<object[], object>)(args => (object)true));

        var authCommand = new AuthCommand(subjectId, action, objectId);
        authCommand.Execute();
    }

    [Fact]
    public void AuthCommand_Throws_When_Unauthorized()
    {
        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        Ioc.Register("Authorization.Check",
            (Func<object[], object>)(args => (object)false));

        var authCommand = new AuthCommand(subjectId, action, objectId);
        var exception = Assert.Throws<UnauthorizedAccessException>(() => authCommand.Execute());
        Assert.Equal("Игрок не имеет прав совершать действие над этим обьектом", exception.Message);
    }

    [Fact]
    public void AuthCommand_Throws_When_AuthCheck_Not_Registered()
    {
        // Ensure Authorization.Check is not registered
        try { Ioc.Unregister("Authorization.Check"); } catch { }

        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        var authCommand = new AuthCommand(subjectId, action, objectId);
        Assert.Throws<InvalidOperationException>(() => authCommand.Execute());
    }

    [Fact]
    public void AuthCommand_Passes_Correct_Parameters_To_AuthCheck()
    {
        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        string? capturedSubjectId = null;
        string? capturedAction = null;
        string? capturedObjectId = null;

        Ioc.Register("Authorization.Check",
            (Func<object[], object>)(args =>
            {
                capturedSubjectId = (string)args[0];
                capturedAction = (string)args[1];
                capturedObjectId = (string)args[2];
                return (object)true;
            }));

        var authCommand = new AuthCommand(subjectId, action, objectId);
        authCommand.Execute();

        Assert.Equal(subjectId, capturedSubjectId);
        Assert.Equal(action, capturedAction);
        Assert.Equal(objectId, capturedObjectId);
    }
}