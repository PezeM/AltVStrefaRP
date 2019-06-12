using AltV.Net;
using System.Timers;
using AltVStrefaRPServer.Models.Enums;
using System;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class TimeManager
    {
        private Timer _gameTimeTimer;
        private readonly int _timerInterval = 60000;
        private int _elapsedMinutes = AppSettings.Current.ServerConfig.ChangeWeatherInterval == 0 
            ? 30 : AppSettings.Current.ServerConfig.ChangeWeatherInterval;
        private readonly Random _rng;

        public uint CurrentWeather { get; set; } = (uint)Weathers.ExtraSunny;
        public GameTime GameTime { get; set; }

        public TimeManager()
        {
            _rng = new Random();
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

            _gameTimeTimer = new Timer();
            _gameTimeTimer.Interval = _timerInterval;
            _gameTimeTimer.Elapsed += GameTimerOnElapsed;
            _gameTimeTimer.AutoReset = true;
            _gameTimeTimer.Start();

            Alt.Log("Time manager initialized.");
        }

        private void GameTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _elapsedMinutes++;
            if (Alt.GetAllPlayers().Count < 1) return;
            UpdateTime();

            if (_elapsedMinutes >= AppSettings.Current.ServerConfig.ChangeWeatherInterval)
            {
                _elapsedMinutes = 0;
                ChangeWeather();
            }
        }

        private void UpdateTime()
        {
            GameTime.Minutes += AppSettings.Current.ServerConfig.OneMinuteIrlToGameTime;
            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetDateTime(GameTime.Days, 0, 0, GameTime.Hours, GameTime.Minutes, 0);
            }
            Alt.Log($"Updated game time to {GameTime}");
        }

        private void ChangeWeather()
        {
            var weatherChance = _rng.Next(0, 101);
            GetNewWeather(weatherChance);

            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetWeather(CurrentWeather);
            }
            Alt.Log($"Updated weather to {CurrentWeather}.");
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
            get { return _hours; }
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
            get { return _minutes; }
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
