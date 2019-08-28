using System.Threading;

namespace AltVStrefaRPServer.Helpers
{
    public class IdGenerator : IIdGenerator
    {
        private int _currentId;

        public int GetNextId()
        {
            return Interlocked.Increment(ref _currentId);
        }

        public void Reset()
        {
            _currentId = 0;
        }
    }
}
