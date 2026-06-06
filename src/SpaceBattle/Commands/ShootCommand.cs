using System;
using System.Linq;

namespace SpaceBattle.Commands
{
    using SpaceBattle;
    using SpaceBattle.Models;
    using SpaceBattle.Repositories;
    using SpaceBattle.Security;

    public sealed class ShootCommand : ICommand
    {
        private readonly string _shooterId;
        private readonly Vector _direction;
        private readonly int _speed;
        private readonly IGameObjectsRepository _repository;
        private readonly IAuthorizer _authorizer;
        private readonly string _callerId;

        public ShootCommand(string shooterId, Vector direction, int speed, IGameObjectsRepository repository, IAuthorizer authorizer, string callerId)
        {
            _shooterId = shooterId ?? throw new ArgumentNullException(nameof(shooterId));
            _direction = direction ?? throw new ArgumentNullException(nameof(direction));
            _speed = speed;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _authorizer = authorizer ?? throw new ArgumentNullException(nameof(authorizer));
            _callerId = callerId ?? throw new ArgumentNullException(nameof(callerId));
        }

        public void Execute()
        {
            if (!_authorizer.CanControl(_callerId, _shooterId))
            {
                return;
            }

            // scale direction by integer speed
            var scaled = new Vector(_direction.Select(c => c * _speed).ToArray());

            // find shooter position
            Vector startPosition = new Vector(0, 0);
            var shooterObj = _repository.GetById(_shooterId);
            if (shooterObj != null)
            {
                startPosition = shooterObj.Position;
            }

            var torpedo = new Torpedo(_shooterId, startPosition, scaled);
            _repository.Add(torpedo);
        }
    }
}
