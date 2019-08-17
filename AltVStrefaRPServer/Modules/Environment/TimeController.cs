using AltV.Net;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class TimeController
    {
        private Timer _gameTimeTimer;
        private readonly int _timerInterval = 60000;
        private int _elapsedMinutes = AppSettings.Current.ServerConfig.ChangeWeatherInterval == 0
            ? 30 : AppSettings.Current.ServerConfig.ChangeWeatherInterval;
        private readonly Random _rng;
        private readonly ILogger<TimeController> _logger;

        public uint CurrentWeather { get; private set; } = (uint)Weathers.ExtraSunny;
        public GameTime GameTime { get; private set; }

        public TimeController(ILogger<TimeController> logger)
        {
            _rng = new Random();
            _logger = logger;
            GameTime = new GameTime
            {
                Days = 0,
                Hours = 12,
                Minutes = 0
            };

            // Update time and weathers for all players on startup
            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetWeather(CurrentWeather);
                player.SetDateTime(GameTime.Days, 0, 0, GameTime.Hours, GameTime.Minutes, 0);
            }

            _gameTimeTimer = new Timer
            {
                Interval = _timerInterval
            };
            _gameTimeTimer.Elapsed += GameTimerOnElapsed;
            _gameTimeTimer.AutoReset = true;
            _gameTimeTimer.Start();

            _logger.LogInformation("Time manager initialized");
        }

        private void GameTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _elapsedMinutes++;
            if (Alt.GetAllPlayers().Count < 1) return;
            UpdateTime();

            if (_elapsedMinutes < AppSettings.Current.ServerConfig.ChangeWeatherInterval)
                return;

            _elapsedMinutes = 0;
            ChangeWeather();
        }

        private void UpdateTime()
        {
            GameTime.Minutes += AppSettings.Current.ServerConfig.OneMinuteIrlToGameTime;
            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetDateTime(GameTime.Days, 0, 0, GameTime.Hours, GameTime.Minutes, 0);
            }

            _logger.LogDebug("Updated game time to {gameTime}", GameTime);
        }

        private void ChangeWeather()
        {
            var weatherChance = _rng.Next(0, 101);
            GetNewWeather(weatherChance);

            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetWeather(CurrentWeather);
            }

            _logger.LogDebug("Updated weather to {weather}", CurrentWeather);
        }

        private void GetNewWeather(int weatherChance)
        {
            if (weatherChance < 50)
            {
                CurrentWeather = (uint)Weathers.Clear;
            }
            else if (weatherChance < 60)
            {
                CurrentWeather = (uint)Weathers.ExtraSunny;
            }
            else if (weatherChance < 70)
            {
                CurrentWeather = (uint)Weathers.Clouds;
            }
            else if (weatherChance < 80)
            {
                CurrentWeather = (uint)Weathers.Smog;
            }
            else if (weatherChance < 85)
            {
                CurrentWeather = (uint)Weathers.Overcast;
            }
            else if (weatherChance < 95)
            {
                CurrentWeather = (uint)Weathers.Lightrain;
            }
            else if (weatherChance <= 100)
            {
                CurrentWeather = (uint)Weathers.Thunderstorm;
            }
        }
    }

    public class GameTime
    {
        private int _hours;
        private int _minutes;

        public int Days { get; set; }

        public int Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                if (_hours >= 24)
                {
                    _hours = 0;
                    Days++;
                }
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                if (_minutes >= 60)
                {
                    _minutes = 0;
                    Hours++;
                }
            }
        }

        public override string ToString() => $"Day: {Days} Hour: {Hours} Minute: {Minutes}";
    }
}
