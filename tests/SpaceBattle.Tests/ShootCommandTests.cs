using System;
using Xunit;

namespace SpaceBattle.Tests
{
    using SpaceBattle.Models;
    using SpaceBattle.Repositories;
    using SpaceBattle.Security;

    public class ShootCommandTests
    {
        [Fact]
        public void ShootCommand_AddsTorpedoToRepository_WhenAuthorized()
        {
            var repo = new InMemoryGameObjectsRepository();
            var authorizer = new SimpleAuthorizer();
            var shooterId = Guid.NewGuid().ToString();

            var ship = new DummyShip(shooterId, new Vector(10, 20), new Vector(0, 0));
            repo.Add(ship);

            var direction = new Vector(1, 0);
            var speed = 100;
            var callerId = shooterId;

            var cmd = new SpaceBattle.Commands.ShootCommand(shooterId, direction, speed, repo, authorizer, callerId);

            cmd.Execute();

            var all = repo.GetAll();
            Assert.Contains(all, o => o is Torpedo);
        }

        [Fact]
        public void ShootCommand_DoesNotAddTorpedo_WhenNotAuthorized()
        {
            var repo = new InMemoryGameObjectsRepository();
            var authorizer = new SimpleAuthorizer();
            var shooterId = Guid.NewGuid().ToString();
            var direction = new Vector(0, 1);
            var cmd = new SpaceBattle.Commands.ShootCommand(shooterId, direction, 100, repo, authorizer, callerId: "someone-else");

            cmd.Execute();

            var all = repo.GetAll();
            Assert.DoesNotContain(all, o => o is Torpedo);
        }

        private class DummyShip : IMovingObject, Models.IHaveId
        {
            public string Id { get; set; }
            public Vector Position { get; set; }
            public Vector Velocity { get; }

            public DummyShip(string id, Vector pos, Vector vel)
            {
                Id = id;
                Position = pos;
                Velocity = vel;
            }
        }
    }
}
