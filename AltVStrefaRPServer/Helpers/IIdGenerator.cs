namespace AltVStrefaRPServer.Helpers
{
    public interface IIdGenerator
    {
        int GetNextId();
        void Reset();
    }
}