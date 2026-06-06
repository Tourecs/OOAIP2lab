using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpaceBattle.Repositories;

public sealed class InMemoryGameObjectsRepository : IGameObjectsRepository
{
    private readonly ConcurrentDictionary<string, IMovingObject> _objects = new();

    public void Add(IMovingObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var idProvider = obj as IHaveId;
        if (idProvider == null || string.IsNullOrWhiteSpace(idProvider.Id))
        {
            if (idProvider != null)
            {
                idProvider.Id = Guid.NewGuid().ToString();
            }
        }

        _objects[idProvider!.Id] = obj;
    }

    public void Remove(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        _objects.TryRemove(id, out _);
    }

    public IReadOnlyCollection<IMovingObject> GetAll() => _objects.Values;

    public IMovingObject? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return _objects.TryGetValue(id, out var obj) ? obj : null;
    }
}
