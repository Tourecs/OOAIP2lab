namespace OOAIP_3lab.GameObjects;

public interface IGameObjectRepository
{
    void Add(IGameObject gameObject);
    void Remove(IGameObject gameObject);
    IEnumerable<IGameObject> GetAll();
    IGameObject GetById(Guid id);
}
