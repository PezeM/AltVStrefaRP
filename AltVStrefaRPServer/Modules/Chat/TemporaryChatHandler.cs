using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Modules.Chat
{
    public class TemporaryChatHandler
    {
        private Dictionary<string, Action<IPlayer,string[]>> _registeredCommands = new Dictionary<string, Action<IPlayer,string[]>>();

        public TemporaryChatHandler()
        {
            Alt.OnClient("chatmessage", ChatMessageEventDelegate);
        }

        private void ChatMessageEventDelegate(IPlayer player, object[] args)
        {
            if (!args[0].ToString().StartsWith('/')) return;
            var message = args[0].ToString().Trim().Remove(0,1).Split(' ');
            if (message.Length <= 0) return;
            var command = message[0];
            string[] cmdArgs = null;
            if (message.Length > 1)
            {
                cmdArgs = message.Skip(1).ToArray();
            }

            InvokeCommand(command, cmdArgs, player);
        }

        private void InvokeCommand(string commandName, string[] args, IPlayer player)
        {
            if (_registeredCommands.ContainsKey(commandName))
            {
                _registeredCommands[commandName].Invoke(player, args);
            }
        }

        public bool RegisterCommand(string commandName, Action<IPlayer,string[]> callback)
        {
            return _registeredCommands.TryAdd(commandName, callback);
        }
    }
}
