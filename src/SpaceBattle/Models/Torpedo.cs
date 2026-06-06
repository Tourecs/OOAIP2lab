using System;

namespace SpaceBattle
{
    using SpaceBattle.Models;

    public sealed class Torpedo : IMovingObject, IHaveId
    {
        public string Id { get; set; }
        public string OwnerId { get; }
        public Vector Position { get; set; }
        public Vector Velocity { get; private set; }
        public bool IsDestroyed { get; private set; }
        public double Lifetime { get; private set; }

        public Torpedo(string ownerId, Vector position, Vector velocity, double lifetimeSeconds = 10.0)
        {
            Id = Guid.NewGuid().ToString();
            OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
            Position = position;
            Velocity = velocity;
            Lifetime = lifetimeSeconds;
            IsDestroyed = false;
        }

        public void Tick(double deltaTime)
        {
            if (IsDestroyed) return;
            Lifetime -= deltaTime;
            if (Lifetime <= 0) IsDestroyed = true;
        }

        public void Destroy() => IsDestroyed = true;
    }
}
