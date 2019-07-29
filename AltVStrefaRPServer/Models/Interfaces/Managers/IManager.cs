namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    // Don't know yet if it will be used
    // May be if we implement methods like TryGetEntity(int id, out TEntity entity) etc
    public interface IManager<TEntity> where TEntity : class
    { }
}
