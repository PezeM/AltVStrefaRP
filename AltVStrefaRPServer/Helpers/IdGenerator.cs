using System.Threading;

namespace AltVStrefaRPServer.Helpers
{
    public class IdGenerator : IIdGenerator
    {
        private int currentId;

        public int GetNextId()
        {
            return Interlocked.Increment(ref currentId);
        }

        public void Reset()
        {
            currentId = 0;
        }
    }
}
