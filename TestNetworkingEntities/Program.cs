using System;
using System.Collections.Generic;
using System.Threading;
using AltV.Net.NetworkingEntity;
using AltV.Net.NetworkingEntity.Elements.Entities;
using Entity;

namespace TestNetworkingEntities
{
    public class Program
    {
        private static Dictionary<ulong, INetworkingEntity> _entities;
        private static Random _rng;

        public static void Main(string[] args)
        {
            _entities = new Dictionary<ulong, INetworkingEntity>();
            _rng = new Random();
            AltNetworking.Configure(options =>
            {
                options.Port = 46429;
            });

            var data = new Dictionary<string, object>();
            var data2 = new Dictionary<string, object>();
            data["bla"] = "123";
            data["bla2"] = 1235;
            var entityToUpdate = AltNetworking.CreateEntity(new Position {X = 0, Y = 0, Z = 0}, 1, 50, data);
            _entities.TryAdd(entityToUpdate.Id, entityToUpdate);
            var secondEntity = AltNetworking.CreateEntity(new Position {X = 1, Y = 1, Z = 1}, 1, 50, data2);
            _entities.TryAdd(secondEntity.Id, secondEntity);
            AltNetworking.OnEntityStreamIn = (entity, client) =>
            {
                Console.WriteLine("streamed in " + entity.Id + " in client " + client.Token);
            };
            AltNetworking.OnEntityStreamOut = (entity, client) =>
            {
                Console.WriteLine("streamed out " + entity.Id + " in client " + client.Token);
            };
            SomethingElse();

            Console.ReadKey();
        }

        private static void SomethingElse()
        {
            int i = 0;
            while (true)
            {
                var randomPosition = GenerateRandomPosition();
                var newEntity = AltNetworking.CreateEntity(new Position { X = randomPosition.X, Y = randomPosition.Y, Z = randomPosition.Z }
                    , 0, _rng.Next(10, 150), GenerateRandomData());
                _entities.TryAdd(newEntity.Id, newEntity);
                Console.WriteLine($"Created entity {newEntity.Id} with size {newEntity.Position.CalculateSize()} on thread {Thread.CurrentThread.ManagedThreadId}");
                i++;
                Thread.Sleep(5000);
            }
        }

        private static Position GenerateRandomPosition()
        {
            var newPosition = new Position
            {
                X = _rng.Next(10, 1000),
                Y = _rng.Next(10, 1000),
                Z = _rng.Next(10, 1000)
            };
            return newPosition;
        }

        private static Dictionary<string, object> GenerateRandomData()
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

