using AltV.Net.Elements.Entities;
using AltV.Net.NetworkingEntity;
using AltV.Net.NetworkingEntity.Elements.Entities;
using AltV.Net.NetworkingEntity.Elements.Pools;
using AltV.Net.NetworkingEntity.Elements.Providers;
using net.vieapps.Components.WebSockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Modules.Networking
{
    public class NonePlayerAuthenticationProvider : IAuthenticationProvider
    {
        private readonly WebSocket webSocket;
        private readonly Dictionary<string, IPlayer> playerTokens = new Dictionary<string, IPlayer>();
        private readonly Dictionary<IPlayer, string> playerTokenAccess = new Dictionary<IPlayer, string>();

        public NonePlayerAuthenticationProvider(string ip, int port, WebSocket webSocket) : this(
            "ws://" + (ip ?? GetIpAddress()) + $":{port}/",
            webSocket)
        {
        }

        public NonePlayerAuthenticationProvider(string url, WebSocket webSocket)
        {
            this.webSocket = webSocket;
            //Alt.OnPlayerConnect += (player, reason) =>
            //{
            //    Task.Run(() =>
            //    {
            //        var client = AltNetworking.CreateClient();
            //        lock (client)
            //        {
            //            if (!client.Exists) return;
            //            lock (playerTokens)
            //            {
            //                playerTokens[client.Token] = player;
            //                playerTokenAccess[player] = client.Token;

            //                player.Emit("streamingToken", url, client.Token);
            //            }
            //        }
            //    });
            //};
            //Alt.OnPlayerDisconnect += (player, reason) =>
            //{
            //    Task.Run(async () =>
            //    {
            //        string token;
            //        lock (playerTokens)
            //        {
            //            if (playerTokenAccess.Remove(player, out token))
            //            {
            //                playerTokens.Remove(token);
            //            }
            //            else
            //            {
            //                return;
            //            }
            //        }

            //        //if (AltNetworking..Remove(token, out var client))
            //        //{
            //        //    var clientWebSocket = client.WebSocket;
            //        //    if (clientWebSocket != null)
            //        //    {
            //        //        await webSocket.CloseWebSocketAsync(clientWebSocket, WebSocketCloseStatus.NormalClosure,
            //        //            "disconnected");
            //        //    }
            //        //}
            //    });
            //};
        }

        public INetworkingClient GetClient(ManagedWebSocket managedWebSocket)
        {
            if (managedWebSocket.Extra.TryGetValue("client", out var clientObj) &&
                clientObj is INetworkingClient client)
            {
                return client;
            }

            return null;
        }

        public void OnConnectionBroken(ManagedWebSocket managedWebSocket)
        {
            managedWebSocket.Extra.Remove("client");
        }

        public void OnConnectionEstablished(ManagedWebSocket managedWebSocket)
        {
        }

        public void OnError(ManagedWebSocket managedWebSocket, Exception exception)
        {
            Console.WriteLine(exception);
        }

        public bool VerifyPosition(INetworkingClient client, INetworkingEntity entity, bool inRange)
        {
            return true;
        }

        public Task<bool> Verify(INetworkingClientPool networkingClientPool, ManagedWebSocket webSocket, string token, out INetworkingClient client)
        {
            if (!networkingClientPool.TryGet(token, out client))
                return Task.FromResult(false);

            client = AltNetworking.CreateClient();
            client.WebSocket = webSocket; //TODO: maybe do that automatically, but we would lost freedom
            webSocket.Extra["client"] = client;
            return Task.FromResult(true);
        }

        private static string GetIpAddress()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            if (ipHostInfo.AddressList.Length == 0) return null;
            var ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }
    }
}
