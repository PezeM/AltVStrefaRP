using AltV.Net.NetworkingEntity.Elements.Factories;
using AltV.Net.NetworkingEntity.Elements.Providers;
using net.vieapps.Components.WebSockets;

namespace AltVStrefaRPServer.Modules.Networking
{
    public class NonePlayerAuthenticationProviderFactory : IAuthenticationProviderFactory
    {
        public IAuthenticationProvider Create(string ip, int port, WebSocket webSocket)
        {
            return new NonePlayerAuthenticationProvider(ip, port, webSocket);
        }
    }
}
