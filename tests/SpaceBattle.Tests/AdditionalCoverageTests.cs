using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;

[Collection("Sequential")]
public sealed class AdditionalCoverageTests : IDisposable
{
    public AdditionalCoverageTests()
    {
        Ioc.Clear();
    }

    public void Dispose()
    {
        Ioc.Clear();
    }

    // === Vector tests ===

    [Fact]
    public void Vector_Constructor_NullCoordinates_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new Vector(null!));
    }

    [Fact]
    public void Vector_Dimensions_ReturnsCorrectCount()
    {
        var v = new Vector(1, 2, 3);
        Assert.Equal(3, v.Dimensions);
    }

    [Fact]
    public void Vector_Indexer_ReturnsCorrectValue()
    {
        var v = new Vector(10, 20, 30);
        Assert.Equal(20, v[1]);
    }

    [Fact]
    public void Vector_ToString_ReturnsFormattedString()
    {
        var v = new Vector(1, 2, 3);
        Assert.Equal("(1, 2, 3)", v.ToString());
    }

    [Fact]
    public void Vector_GetEnumerator_IteratesCorrectly()
    {
        var v = new Vector(5, 6, 7);
        var list = new List<int>();
        foreach (var coord in v)
            list.Add(coord);
        Assert.Equal([5, 6, 7], list);
    }

    [Fact]
    public void Vector_OperatorPlus_NullLeft_Throws()
    {
        Vector? left = null;
        Assert.Throws<ArgumentNullException>(() => left! + new Vector(1));
    }

    [Fact]
    public void Vector_OperatorPlus_NullRight_Throws()
    {
        Vector? right = null;
        Assert.Throws<ArgumentNullException>(() => new Vector(1) + right!);
    }

    [Fact]
    public void Vector_OperatorPlus_DifferentDimensions_Throws()
    {
        var a = new Vector(1, 2);
        var b = new Vector(1, 2, 3);
        Assert.Throws<ArgumentException>(() => a + b);
    }

    [Fact]
    public void Vector_Equality_NullOther_ReturnsFalse()
    {
        var v = new Vector(1, 2);
        Assert.False(v.Equals((Vector?)null));
    }

    [Fact]
    public void Vector_Equality_DifferentObject_ReturnsFalse()
    {
        var v = new Vector(1, 2);
        Assert.False(v.Equals("not a vector"));
    }

    [Fact]
    public void Vector_OperatorEquality_BothNull_ReturnsTrue()
    {
        Vector? a = null;
        Vector? b = null;
        Assert.True(a == b);
    }

    [Fact]
    public void Vector_OperatorInequality_SameVector_ReturnsFalse()
    {
        var v = new Vector(1, 2);
        Assert.False(v != v);
    }

    // === Angle tests ===

    [Fact]
    public void Angle_Constructor_WrongDenominator_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Angle(1, 4));
    }

    [Fact]
    public void Angle_OperatorPlus_NullLeft_Throws()
    {
        Angle? left = null;
        Assert.Throws<ArgumentNullException>(() => left! + new Angle(1));
    }

    [Fact]
    public void Angle_OperatorPlus_NullRight_Throws()
    {
        Angle? right = null;
        Assert.Throws<ArgumentNullException>(() => new Angle(1) + right!);
    }

    [Fact]
    public void Angle_ToString_ReturnsFormattedString()
    {
        var a = new Angle(3);
        Assert.Equal("(3, 8)", a.ToString());
    }

    [Fact]
    public void Angle_ImplicitToDouble_ReturnsCorrectRadians()
    {
        var a = new Angle(2);
        double d = a;
        Assert.Equal(Math.PI / 2, d, 0.0001);
    }

    [Fact]
    public void Angle_ImplicitToDouble_Null_Throws()
    {
        Angle? a = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            double d = a!;
        });
    }

    // === Ioc tests ===

    [Fact]
    public void Ioc_Register_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Ioc.Register(null!, _ => "value"));
    }

    [Fact]
    public void Ioc_Register_EmptyKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => Ioc.Register("", _ => "value"));
    }

    [Fact]
    public void Ioc_Register_NullDependency_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Ioc.Register("key", null!));
    }

    [Fact]
    public void Ioc_Resolve_NullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Ioc.Resolve<object>(null!));
    }

    [Fact]
    public void Ioc_Clear_RemovesAllDependencies()
    {
        Ioc.Register("Test.Clear", _ => "value");
        Assert.Equal("value", Ioc.Resolve<string>("Test.Clear"));
        Ioc.Clear();
        Assert.Throws<InvalidOperationException>(() => Ioc.Resolve<string>("Test.Clear"));
    }

    // === MacroCommand tests ===

    [Fact]
    public void MacroCommand_NullCommands_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MacroCommand(null!));
    }

    // === CommandInjectableCommand tests ===

    [Fact]
    public void CommandInjectable_Inject_Null_Throws()
    {
        var cmd = new CommandInjectableCommand();
        Assert.Throws<ArgumentNullException>(() => cmd.Inject(null!));
    }

    [Fact]
    public void CommandInjectable_Execute_WithoutInject_Throws()
    {
        var cmd = new CommandInjectableCommand();
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    // === StartLongOperationCommand tests ===

    [Fact]
    public void StartLongOperation_NullOrder_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new StartLongOperationCommand(null!));
    }

    [Fact]
    public void StartLongOperation_MissingOperationKey_Throws()
    {
        var order = new Dictionary<string, object>();
        var cmd = new StartLongOperationCommand(order);
        Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
    }

    [Fact]
    public void StartLongOperation_MissingReceiverKey_Throws()
    {
        var order = new Dictionary<string, object> { ["operation"] = "Move", ["object"] = new object() };
        var cmd = new StartLongOperationCommand(order);
        Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
    }

    // === StopLongOperationCommand tests ===

    [Fact]
    public void StopLongOperation_NullOrder_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new StopLongOperationCommand(null!));
    }

    [Fact]
    public void StopLongOperation_MissingCommandKey_Throws()
    {
        var order = new Dictionary<string, object>();
        var cmd = new StopLongOperationCommand(order);
        Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
    }

    // === AuthCommand + RegisterIoCDependencyAuthCheck tests ===

    [Fact]
    public void AuthCommand_Unauthorized_Throws()
    {
        Ioc.Register("Authorization.Check", _ => false);
        var cmd = new AuthCommand("player1", "Fire", "ship1");
        var ex = Assert.Throws<UnauthorizedAccessException>(() => cmd.Execute());
        Assert.Contains("не имеет прав", ex.Message);
    }

    [Fact]
    public void AuthCommand_Authorized_Succeeds()
    {
        Ioc.Register("Authorization.Check", _ => true);
        var cmd = new AuthCommand("player1", "Fire", "ship1");
        cmd.Execute(); // no exception
    }

    [Fact]
    public void RegisterIoCDependencyAuthCheck_GlobalWildcard_PermissionGranted()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            ["*"] = new List<string> { "Move" }
        };
        Ioc.Register("Authorization.GetPermissions", _ => permissions);
        new RegisterIoCDependencyAuthCheck().Execute();

        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "any_object");
        Assert.True(result);
    }

    [Fact]
    public void RegisterIoCDependencyAuthCheck_ObjectWildcard_PermissionGranted()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            ["ship1"] = new List<string> { "*" }
        };
        Ioc.Register("Authorization.GetPermissions", _ => permissions);
        new RegisterIoCDependencyAuthCheck().Execute();

        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Anything", "ship1");
        Assert.True(result);
    }

    [Fact]
    public void RegisterIoCDependencyAuthCheck_ObjectSpecificAction_PermissionGranted()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            ["ship1"] = new List<string> { "Move" }
        };
        Ioc.Register("Authorization.GetPermissions", _ => permissions);
        new RegisterIoCDependencyAuthCheck().Execute();

        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship1");
        Assert.True(result);
    }

    [Fact]
    public void RegisterIoCDependencyAuthCheck_ObjectSpecificAction_Denied()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            ["ship1"] = new List<string> { "Move" }
        };
        Ioc.Register("Authorization.GetPermissions", _ => permissions);
        new RegisterIoCDependencyAuthCheck().Execute();

        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Fire", "ship1");
        Assert.False(result);
    }

    [Fact]
    public void RegisterIoCDependencyAuthCheck_UnknownObject_Denied()
    {
        var permissions = new Dictionary<string, IEnumerable<string>>
        {
            ["ship1"] = new List<string> { "Move" }
        };
        Ioc.Register("Authorization.GetPermissions", _ => permissions);
        new RegisterIoCDependencyAuthCheck().Execute();

        var result = Ioc.Resolve<bool>("Authorization.Check", "player1", "Move", "ship_unknown");
        Assert.False(result);
    }

    // === FireCommand edge cases ===

    [Fact]
    public void FireCommand_MissingPositionKey_Throws()
    {
        Ioc.Register("Adapters.IMovingObject", args =>
        {
            var data = (IDictionary<string, object>)args[0];
            return new MovingObjectAdapter(data);
        });
        Ioc.Register("Adapters.IRotatingObject", args => Mock.Of<IRotatingObject>());

        var ship = new Dictionary<string, object>(); // no Position key
        var cmd = new FireCommand(ship);
        Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
    }

    // === MovingObjectAdapter tests ===

    [Fact]
    public void MovingObjectAdapter_PositionSetter_UpdatesDictionary()
    {
        var data = new Dictionary<string, object>
        {
            ["Position"] = new Vector(1, 2),
            ["Velocity"] = new Vector(3, 4)
        };
        var adapter = new MovingObjectAdapter(data);
        adapter.Position = new Vector(5, 6);
        Assert.Equal(new Vector(5, 6), (Vector)data["Position"]);
    }

    [Fact]
    public void MovingObjectAdapter_MissingPositionKey_Throws()
    {
        var data = new Dictionary<string, object> { ["Velocity"] = new Vector(1) };
        var adapter = new MovingObjectAdapter(data);
        Assert.Throws<KeyNotFoundException>(() => _ = adapter.Position);
    }

    [Fact]
    public void MovingObjectAdapter_MissingVelocityKey_Throws()
    {
        var data = new Dictionary<string, object> { ["Position"] = new Vector(1) };
        var adapter = new MovingObjectAdapter(data);
        Assert.Throws<KeyNotFoundException>(() => _ = adapter.Velocity);
    }

    // === AdapterRegistration tests ===

    [Fact]
    public void AdapterRegistration_WrongType_Throws()
    {
        AdapterRegistration.Register();
        Assert.Throws<ArgumentException>(() =>
            Ioc.Resolve<IMovingObject>("Adapters.IMovingObject", "not a dict"));
    }

    // === GameRegistry tests ===

    [Fact]
    public void AddObjectToRegistry_DuplicateId_Throws()
    {
        Ioc.Register("Game.Registry", _ => new Dictionary<Guid, IDictionary<string, object>>());
        new RegisterIoCDependencyGameRegistry().Execute();
        var registry = Ioc.Resolve<Dictionary<Guid, IDictionary<string, object>>>("Game.Registry");

        var id = Guid.NewGuid();
        var obj = new Dictionary<string, object>();
        registry[id] = obj;

        var cmd = new AddObjectToRegistryCommand(id, obj);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void DeleteObjectFromRegistry_MissingId_Throws()
    {
        Ioc.Register("Game.Registry", _ => new Dictionary<Guid, IDictionary<string, object>>());
        new RegisterIoCDependencyGameRegistry().Execute();

        var missingId = Guid.NewGuid();
        var cmd = new DeleteObjectFromRegistryCommand(missingId);
        Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
    }

    // === Game exception handler ===

    [Fact]
    public void Game_Execute_ExceptionHandlerIsCalled()
    {
        var queue = new Queue<ICommand>();
        var failingCmd = new Mock<ICommand>();
        failingCmd.Setup(c => c.Execute()).Throws<Exception>();

        queue.Enqueue(failingCmd.Object);
        Ioc.Register("Game.Queue.Take", _ => queue.Dequeue());
        Ioc.Register("Game.Queue.Count", _ => (Func<int>)(() => queue.Count));
        Ioc.Register("Command.Time", _ => TimeSpan.FromMinutes(1));

        var handler = new Mock<ICommand>();
        Ioc.Register("ExceptionHandler", _ => handler.Object);

        var game = new Game();
        game.Execute();

        handler.Verify(h => h.Execute(), Times.Once);
    }

    // === CreateMacroCommandStrategy tests ===

    [Fact]
    public void CreateMacroCommandStrategy_NullSpec_Throws()
    {
        Assert.Throws<ArgumentException>(() => new CreateMacroCommandStrategy(null!));
    }

    [Fact]
    public void CreateMacroCommandStrategy_EmptySpec_Throws()
    {
        Assert.Throws<ArgumentException>(() => new CreateMacroCommandStrategy(""));
    }

    [Fact]
    public void CreateMacroCommandStrategy_NullArgs_Throws()
    {
        Ioc.Register("Specs.Test", _ => new[] { "Commands.Move" });
        Ioc.Register("Commands.Move", _ => Mock.Of<ICommand>());
        var strategy = new CreateMacroCommandStrategy("Test");
        Assert.Throws<ArgumentNullException>(() => strategy.Resolve(null!));
    }
}