using AltV.Net;
using AltV.Net.Elements.Entities;
using System;

namespace AltVStrefaRPServer.Modules.Chat
{
    public class ChatHandler
    {
        private readonly Action<string> broadcast;

        private readonly Action<IPlayer, string> send;

        private readonly Action<string, Action<IPlayer, string[]>> registerCmd;

        public ChatHandler()
        {
            Alt.Import("chat", "broadcast", out broadcast);
            Alt.Import("chat", "send", out send);
            Alt.Import("chat", "registerCmd", out registerCmd);
        }

        public void Broadcast(string message)
        {
            broadcast?.Invoke(message);
        }

        public void Send(IPlayer player, string message)
        {
            send?.Invoke(player, message);
        }

        public void RegisterCommand(string command, Action<IPlayer, string[]> callback)
        {
            try
            {
                registerCmd?.Invoke(command, callback);
            }
            catch (Exception e)
            {
                Alt.Log(e.ToString());
                throw;
            }
        }
    }
}
