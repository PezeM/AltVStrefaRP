using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Core
{
    public class SoundManager
    {
        private readonly IIdGenerator _idGenerator;

        public SoundManager()
        {
            _idGenerator = new IdGenerator();
            Alt.On<IStrefaPlayer, string, int, int, Position, bool>("SoundsPlaySoundInRange", PlaySoundInRange);
        }

        private void PlaySoundInRange(IStrefaPlayer strefaPlayer, string soundName, int volume, int range, Position position, bool loop)
        {
            int soundId = _idGenerator.GetNextId();
            foreach (IPlayer player in Alt.GetAllPlayers())
            {
                if (player.Position.Distance(position) < range)
                {
                    player.Emit("soundsPlaySoundInRange", soundId, soundName, volume, loop);
                }
            }
        }
    }
}
