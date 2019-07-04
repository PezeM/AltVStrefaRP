using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using Timer = System.Timers.Timer;

namespace AltVStrefaRPServer.Modules.Networking
{
    //public class TestNetworking
    //{
    //    //public ConcurrentDictionary<ulong, INetworkingEntity> Entities { get; set; }
    //    private Random _rng;
    //    private Timer _testTimer;
    //    public TestNetworking()
    //    {
    //        _rng = new Random();
    //        //Entities = new ConcurrentDictionary<ulong, INetworkingEntity>();
    //        //AltNetworking.Configure(options =>
    //        //{

    //        //});
    //        //Alt.Log("Initialized TestNetworking");
    //        //var someEntity = AltNetworking.CreateEntity(new Position {X = -82, Y = -109, Z = 62}, 0, 50, new Dictionary<string, object>
    //        //{
    //        //    { "someData", 2331 },
    //        //});
    //        //Entities.TryAdd(someEntity.Id, someEntity);
    //        //AltNetworking.OnEntityStreamIn = OnEntityStreamIn;
    //        //AltNetworking.OnEntityStreamOut = OnEntityStreamOut;

    //        //SetupTimer();
    //        Task.Run(() => { SomethingElse(); });
    //    }

    //    private void SomethingElse()
    //    {
    //        int i = 0;
    //        while (true)
    //        {
    //            var player = Alt.GetAllPlayers().FirstOrDefault();
    //            if (player != null) break;
    //            var randomPosition = GenerateRandomPosition();
    //            var newEntity = AltNetworking.CreateEntity(new Position{X = randomPosition.X, Y = randomPosition.Y, Z = randomPosition.Z}
    //                , 0, _rng.Next(10, 150), GenerateRandomData());
    //            Entities.TryAdd(newEntity.Id, newEntity);
    //            Alt.Log($"Created entity {newEntity.Id} on thread {Thread.CurrentThread.ManagedThreadId}");
    //            i++;
    //            if (i > 1000) break;
    //            Thread.Sleep(5000);
    //        }
    //    }

    //    public void SetupTimer()
    //    {
    //        _testTimer = new Timer(3500);
    //        _testTimer.Elapsed += OnTimer;
    //        _testTimer.AutoReset = true;
    //        _testTimer.Start();
    //    }

    //    private void OnTimer(object sender, ElapsedEventArgs e)
    //    {
    //        try
    //        {
    //            //var player = Alt.GetAllPlayers().FirstOrDefault();
    //            //if (player == null || !player.Exists || player.Position == null) return;
    //            var randomPosition = GenerateRandomPosition();
    //            var newEntity = AltNetworking.CreateEntity(new Position{X = randomPosition.X, Y = randomPosition.Y, Z = randomPosition.Z}
    //                , 0, _rng.Next(10, 150), GenerateRandomData());
    //            Entities.TryAdd(newEntity.Id, newEntity);
    //            Alt.Log($"Created entity {newEntity.Id} on thread {Thread.CurrentThread.ManagedThreadId}");
    //        }
    //        catch (Exception exception)
    //        {
    //            Console.WriteLine(exception);
    //            throw;
    //        }
    //    }

    //    private void OnEntityStreamOut(INetworkingEntity entity, INetworkingClient client)
    //    {
    //        Alt.Log($"Entity streamed out {entity.Id} in client {client.Token}");
    //        DisplayInfo(entity, client);
    //    }

    //    private void OnEntityStreamIn(INetworkingEntity entity, INetworkingClient client)
    //    {
    //        Alt.Log($"Entity streamed in {entity.Id} in client {client.Token}");
    //        DisplayInfo(entity, client);

    //    }

    //    private void DisplayInfo(INetworkingEntity entity, INetworkingClient client)
    //    {
    //        var stringBuilder = new StringBuilder();
    //        stringBuilder.AppendLine($"Streamed clients: ");
    //        foreach (var entityStreamedInClient in entity.StreamedInClients)
    //        {
    //            stringBuilder.AppendLine($"Token: {entityStreamedInClient.Token} Exists: {entityStreamedInClient.Exists} " +
    //                                     $"LocalEndPoint: {entityStreamedInClient.WebSocket.LocalEndPoint} RemoteEndPoint: {entityStreamedInClient.WebSocket.RemoteEndPoint}");
    //        }
    //        stringBuilder.AppendLine($"Entity data: MainStreamer {entity.MainStreamer.Token} Range {entity.Range} PositionSize {entity.Position.CalculateSize()}");
    //        stringBuilder.AppendLine();
    //        Alt.Log(stringBuilder.ToString());
    //    }

    //    private AltV.Net.Data.Position GenerateRandomPosition()
    //    {
    //        var newPosition = new AltV.Net.Data.Position
    //        {
    //            X = _rng.Next(10, 1000),
    //            Y = _rng.Next(10, 1000),
    //            Z = _rng.Next(10,1000)
    //        };
    //        return newPosition;
    //    }

    //    private Dictionary<string, object> GenerateRandomData()
    //    {
    //        var dictionary = new Dictionary<string, object> 
    //        { 
    //            { "someData", _rng.Next(1000,3000)}, 
    //            { "propModel", "somePropModel" },
    //            { "quantity", _rng.Next(1, 10)},
    //            { "canPickup", true}
    //        };
    //        return dictionary;
    //    }
    //}
}
