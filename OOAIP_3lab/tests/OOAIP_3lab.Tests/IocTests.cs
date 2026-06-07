using Moq;
using Xunit;

namespace OOAIP_3lab.Tests;

public class IocTests
{
    [Fact]
    public void IocRegisterAndResolve()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Test.Key",
            (Func<object[], object>)(args => "test value")).Execute();
        var result = Ioc.Resolve<string>("Test.Key");
        Assert.Equal("test value", result);
    }

    [Fact]
    public void IocResolveThrowsWhenKeyNotFound()
    {
        Assert.Throws<ArgumentException>(() => Ioc.Resolve<object>("Nonexistent.Key" + Guid.NewGuid()));
    }

    [Fact]
    public void RegisterIoCDependencyMoveCommandRegisters()
    {
        var mockMovable = new Mock<IMovingObject>();
        mockMovable.Setup(m => m.Position).Returns(new Vector(0, 0));
        mockMovable.Setup(m => m.Velocity).Returns(new Vector(1, 1));
        mockMovable.SetupSet(m => m.Position = It.IsAny<Vector>());

        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IMovingObject",
            (object[] args) => mockMovable.Object).Execute();

        new RegisterIoCDependencyMoveCommand().Execute();
        var rawObj = new Dictionary<string, object>();
        var cmd = Ioc.Resolve<ICommand>("Commands.Move", rawObj);
        Assert.IsType<MoveCommand>(cmd);
    }

    [Fact]
    public void RegisterIoCDependencyRotateCommandRegisters()
    {
        Angle.Denominator = 360;
        var mockRotatable = new Mock<IRotatingObject>();
        mockRotatable.Setup(m => m.Angle).Returns(new Angle(0, 360));
        mockRotatable.Setup(m => m.AngularVelocity).Returns(new Angle(5, 360));
        mockRotatable.SetupSet(m => m.Angle = It.IsAny<Angle?>());

        Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IRotatingObject",
            (object[] args) => mockRotatable.Object).Execute();

        new RegisterIoCDependencyRotateCommand().Execute();
        var rawObj = new Dictionary<string, object>();
        var cmd = Ioc.Resolve<ICommand>("Commands.Rotate", rawObj);
        Assert.IsType<RotateCommand>(cmd);
    }

    [Fact]
    public void RegisterIoCDependencyMacroCommandRegisters()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
        var cmds = new ICommand[] { new EmptyCommand(), new EmptyCommand() };
        var macro = Ioc.Resolve<ICommand>("Macro.Create", new object[] { cmds });
        Assert.IsType<MacroCommand>(macro);
    }

    [Fact]
    public void IocClearResetsStrategies()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Test.Clear",
            (Func<object[], object>)(args => "value")).Execute();
        var result = Ioc.Resolve<string>("Test.Clear");
        Assert.Equal("value", result);
        Ioc.Clear();
        Assert.Throws<ArgumentException>(() => Ioc.Resolve<string>("Test.Clear"));
    }
}