using Xunit;
using System;
using SpaceBattle.Lib.Command;

namespace SpaceBattle.Tests.CommandTests;

public class AuthCommandTests
{
    public AuthCommandTests()
    {
        // Очищаем IoC перед каждым тестом, чтобы они были изолированы
        Ioc.Clear();
    }

    [Fact]
    public void AuthCommand_Successfully_Authorizes()
    {
        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        
        // Регистрируем стратегию проверки, которая возвращает true
        Ioc.Register("Authorization.Check", (Func<object[], object>)(args => true));

        var authCommand = new AuthCommand(subjectId, action, objectId);
        
        // Действие должно пройти без исключений
        authCommand.Execute(); 
    }

    [Fact]
    public void AuthCommand_Throws_When_Unauthorized()
    {
        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        
        // Регистрируем стратегию проверки, которая возвращает false
        Ioc.Register("Authorization.Check", (Func<object[], object>)(args => false));

        var authCommand = new AuthCommand(subjectId, action, objectId);
        
        // Проверяем выброс исключения и текст ошибки (покрытие ветки if)
        var exception = Assert.Throws<UnauthorizedAccessException>(() => authCommand.Execute());
        Assert.Equal("Игрок не имеет прав совершать действие над этим обьектом", exception.Message);
    }

    [Fact]
    public void AuthCommand_Throws_When_AuthCheck_Not_Registered()
    {
        // Гарантируем, что в контейнере нет зависимости "Authorization.Check"
        Ioc.Clear();
        
        var subjectId = "player1";
        var action = "Move";
        var objectId = "ship1";
        var authCommand = new AuthCommand(subjectId, action, objectId);
        
        // Так как зависимость не зарегистрирована, Ioc.Resolve выбросит исключение.
        // Это заменяет некорректный тест с ушедшим методом Unregister.
        Assert.Throws<Exception>(() => authCommand.Execute());
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

        // Регистрируем стратегию и перехватываем аргументы, которые в неё передаются
        Ioc.Register("Authorization.Check", (Func<object[], object>)(args =>
        {
            capturedSubjectId = (string)args[0];
            capturedAction = (string)args[1];
            capturedObjectId = (string)args[2];
            return true;
        }));

        var authCommand = new AuthCommand(subjectId, action, objectId);
        authCommand.Execute();

        // Проверяем, что команда передала в IoC именно те параметры, с какими была создана
        Assert.Equal(subjectId, capturedSubjectId);
        Assert.Equal(action, capturedAction);
        Assert.Equal(objectId, capturedObjectId);
    }
}
