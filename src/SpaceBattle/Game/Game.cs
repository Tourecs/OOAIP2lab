using System;
using System.Linq;

namespace SpaceBattle
{
    using SpaceBattle.Repositories;
    using SpaceBattle.Security;
    using SpaceBattle.Models;

    public sealed class Game
    {
        private readonly IGameObjectsRepository _repository;
        private readonly IAuthorizer _authorizer;

        public double MinX { get; } = 0;
        public double MinY { get; } = 0;
        public double MaxX { get; } = 1000;
        public double MaxY { get; } = 1000;

        public Game(IGameObjectsRepository repository, IAuthorizer authorizer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _authorizer = authorizer ?? throw new ArgumentNullException(nameof(authorizer));
        }

        public void ProcessCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            command.Execute();
        }

        public void Update(double deltaTime)
        {
            if (deltaTime <= 0) return;

            var all = _repository.GetAll().ToArray();
            foreach (var obj in all)
            {
                try
                {
                    obj.Position = obj.Position + obj.Velocity;
                }
                catch
                {
                }

                if (obj is Torpedo t)
                {
                    t.Tick(deltaTime);
                    if (t.IsDestroyed || IsOutOfBounds(t.Position))
                    {
                        _repository.Remove(t.Id);
                    }
                }
                else
                {
                    if (IsOutOfBounds(obj.Position) && obj is Models.IHaveId haveId)
                    {
                        _repository.Remove(haveId.Id);
                    }
                }
            }
        }

        private bool IsOutOfBounds(Vector pos)
        {
            return pos[0] < MinX || pos[0] > MaxX || pos[1] < MinY || pos[1] > MaxY;
        }
    }
}
