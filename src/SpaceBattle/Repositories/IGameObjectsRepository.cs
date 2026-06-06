using System.Collections.Generic;

namespace SpaceBattle.Repositories;

public interface IGameObjectsRepository
{
    void Add(IMovingObject obj);
    void Remove(string id);
    IReadOnlyCollection<IMovingObject> GetAll();
    IMovingObject? GetById(string id);
}
