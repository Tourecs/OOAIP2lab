using Moq;
using Xunit;

namespace OOAIP_3lab.Tests;

public class AuthCommandTests
{
    [Fact]
    public void AuthCommandSuccessfullyAuthorizes()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
            new Func<object[], object>((object[] args) => (object)true)).Execute();

        var authCommand = new AuthCommand("player1", "Move", "ship1");
        authCommand.Execute();
    }

    [Fact]
    public void AuthCommandThrowsWhenUnauthorized()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
            new Func<object[], object>((object[] args) => (object)false)).Execute();

        var authCommand = new AuthCommand("player1", "Move", "ship1");
        var exception = Assert.Throws<UnauthorizedAccessException>(() => authCommand.Execute());
        Assert.Equal("Игрок не имеет прав совершать действие над этим объектом", exception.Message);
    }

    [Fact]
    public void AuthCommandPassesCorrectParametersToAuthCheck()
    {
        string? capturedSubjectId = null;
        string? capturedAction = null;
        string? capturedObjectId = null;

        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
            new Func<object[], object>((object[] args) =>
            {
                capturedSubjectId = (string)args[0];
                capturedAction = (string)args[1];
                capturedObjectId = (string)args[2];
                return (object)true;
            })).Execute();

        var authCommand = new AuthCommand("player1", "Move", "ship1");
        authCommand.Execute();

        Assert.Equal("player1", capturedSubjectId);
        Assert.Equal("Move", capturedAction);
        Assert.Equal("ship1", capturedObjectId);
    }
}