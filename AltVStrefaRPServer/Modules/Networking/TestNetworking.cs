using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using AltV.Net.NetworkingEntity;
using AltV.Net.NetworkingEntity.Elements.Entities;
using Timer = System.Timers.Timer;
using Entity;

namespace AltVStrefaRPServer.Modules.Networking
{
    public class TestNetworking
    {
        public ConcurrentDictionary<ulong, INetworkingEntity> Entities { get; set; }
        private Random _rng;
        private Timer _testTimer;
        public TestNetworking()
        {
            _rng = new Random();
            Entities = new ConcurrentDictionary<ulong, INetworkingEntity>();
            AltNetworking.Configure(options =>
            {
                options.Port = 46429;
                options.Ip = "127.0.0.1";
                //options.AuthenticationProviderFactory = new NonePlayerAuthenticationProviderFactory();
            });
            Alt.Log("Initialized TestNetworking");
            var someEntity = AltNetworking.CreateEntity(new Position { X = -82, Y = -109, Z = 62 }, 1, 500, new Dictionary<string, object>
            {
                { "someData", 2331 },
            });
            Entities.TryAdd(someEntity.Id, someEntity);
            AltNetworking.OnEntityStreamIn = OnEntityStreamIn;
            AltNetworking.OnEntityStreamOut = OnEntityStreamOut;
            SetupTimer();
        }

        public void SetupTimer()
        {
            _testTimer = new Timer(1000);
            _testTimer.Elapsed += OnTimer;
            _testTimer.AutoReset = true;
            _testTimer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                var player = Alt.GetAllPlayers().FirstOrDefault();
                if (player == null) return;
                var randomPosition = GenerateRandomPosition(player.Position);
                var newEntity = AltNetworking.CreateEntity(new Position { X = player.Position.X, Y = player.Position.Y, Z = player.Position.Z }
                    , _rng.Next(0, 2), _rng.Next(100, 150), GenerateRandomData());
                Entities.TryAdd(newEntity.Id, newEntity);
                Alt.Log($"Created entity {newEntity.Id} dim {newEntity.Dimension} on thread {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (NullReferenceException exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void OnEntityStreamOut(INetworkingEntity entity, INetworkingClient client)
        {
            Alt.Log($"Entity streamed out {entity.Id} in client {client.Token}");
            DisplayInfo(entity, client);
        }

        private void OnEntityStreamIn(INetworkingEntity entity, INetworkingClient client)
        {
            Alt.Log($"Entity streamed in {entity.Id} in client {client.Token}");
            DisplayInfo(entity, client);

        }

        private void DisplayInfo(INetworkingEntity entity, INetworkingClient client)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Streamed clients: ");
            foreach (var entityStreamedInClient in entity.StreamedInClients)
            {
                stringBuilder.AppendLine($"Token: {entityStreamedInClient.Token} Exists: {entityStreamedInClient.Exists} " +
                                         $"LocalEndPoint: {entityStreamedInClient.WebSocket.LocalEndPoint} RemoteEndPoint: {entityStreamedInClient.WebSocket.RemoteEndPoint}");
            }
            stringBuilder.AppendLine($"Entity data: MainStreamer {entity.MainStreamer.Token} Range {entity.Range} PositionSize {entity.Position.CalculateSize()}");
            stringBuilder.AppendLine();
            Alt.Log(stringBuilder.ToString());
        }

        private AltV.Net.Data.Position GenerateRandomPosition(AltV.Net.Data.Position position)
        {
            var newPosition = new AltV.Net.Data.Position
            {
                X = position.X + _rng.Next(1, 10),
                Y = position.Y + _rng.Next(1, 10),
                Z = position.Z + _rng.Next(1, 10)
            };
            return newPosition;
        }

        private Dictionary<string, object> GenerateRandomData()
        {
            var dictionary = new Dictionary<string, object>
            {
                { "someData", _rng.Next(1000,3000)},
                { "propModel", "somePropModel" },
                { "quantity", _rng.Next(1, 10)},
                { "canPickup", true}
            };
            return dictionary;
        }
    }
}
