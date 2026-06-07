using Xunit;

namespace OOAIP_3lab.Tests;

public class RegisterIoCDependencyAuthCheckTests
{
    [Fact]
    public void RegisterIoCDependencyAuthCheckSuccessfullyRegisters()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            { "ship1", new List<string> { "Action" } }
        };
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.GetPermissions",
            new Func<object[], object>((object[] args) => (object)permissions)).Execute();

        new RegisterIoCDependencyAuthCheck().Execute();
        Assert.NotNull(Ioc.Resolve<object>("Authorization.Check", "player1", "Action", "object1"));
    }

    [Fact]
    public void AuthCheckReturnsTrueWhenPlayerHasGlobalPermissions()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            { "*", new List<string> { "Move", "Fire" } }
        };
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.GetPermissions",
            new Func<object[], object>((object[] args) => (object)permissions)).Execute();

        new RegisterIoCDependencyAuthCheck().Execute();
        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship1");
        Assert.True(result);
    }

    [Fact]
    public void AuthCheckReturnsFalseWhenObjectNotInPermissions()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            { "ship2", new List<string> { "Move", "Fire" } }
        };
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.GetPermissions",
            new Func<object[], object>((object[] args) => (object)permissions)).Execute();

        new RegisterIoCDependencyAuthCheck().Execute();
        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship1");
        Assert.False(result);
    }

    [Fact]
    public void AuthCheckReturnsTrueWhenObjectHasWildcardPermission()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            { "ship1", new List<string> { "*" } }
        };
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.GetPermissions",
            new Func<object[], object>((object[] args) => (object)permissions)).Execute();

        new RegisterIoCDependencyAuthCheck().Execute();
        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship1");
        Assert.True(result);
    }

    [Fact]
    public void AuthCheckReturnsTrueWhenObjectHasSpecificPermission()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            { "ship1", new List<string> { "Move", "Fire" } }
        };
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.GetPermissions",
            new Func<object[], object>((object[] args) => (object)permissions)).Execute();

        new RegisterIoCDependencyAuthCheck().Execute();
        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship1");
        Assert.True(result);
    }

    [Fact]
    public void AuthCheckReturnsFalseWhenObjectDoesNotHaveSpecificPermission()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            { "ship1", new List<string> { "Fire", "Repair" } }
        };
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.GetPermissions",
            new Func<object[], object>((object[] args) => (object)permissions)).Execute();

        new RegisterIoCDependencyAuthCheck().Execute();
        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship1");
        Assert.False(result);
    }
}