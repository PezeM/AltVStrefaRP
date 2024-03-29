﻿namespace AltVStrefaRPServer.Models.Server
{
    public class AppSettings
    {
        public static AppSettings Current;

        public void Initialize()
        {
            Current = this;
        }

        public string ConnectionString { get; set; }
        public ElasticsearchOptions ElasticsearchOptions { get; set; }
        public ServerConfig ServerConfig { get; set; }
        public string VehiclesDataPath { get; set; }
    }

    public class ServerConfig
    {
        public NetworkingManagerConfig NetworkingManagerConfig { get; set; }
        public ConfigPosition SpawnPosition { get; set; }
        public ConfigPosition LoginPosition { get; set; }
        public EconomySettings EconomySettings { get; set; }
        public float StartingPlayerMoney { get; set; } = 250.0f;
        public int ChangeWeatherInterval { get; set; } = 30;
        public int OneMinuteIrlToGameTime { get; set; } = 20;
        public int WeatherTransitionDuration { get; set; } = 60;
    }

    public class ConfigPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }
    }
}