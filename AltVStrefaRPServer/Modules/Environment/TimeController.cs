using AltV.Net;
using AltVStrefaRPServer.Models.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;
using AltVStrefaRPServer.Models.Environment.Weathers;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class TimeController
    {
        private readonly Timer _gameTimeTimer;
        private const int TimerInterval = 60000;

        private readonly int _weatherTransitionDuration =
            AppSettings.Current.ServerConfig.WeatherTransitionDuration == 0
                ? 10_000
                : AppSettings.Current.ServerConfig.WeatherTransitionDuration;
        private int _elapsedMinutes = AppSettings.Current.ServerConfig.ChangeWeatherInterval == 0
            ? 30 : AppSettings.Current.ServerConfig.ChangeWeatherInterval;
        private readonly Random _rng;
        private readonly ILogger<TimeController> _logger;

        public IWeather CurrentWeather { get; private set; }
        public GameTime GameTime { get; private set; }

        public TimeController(ILogger<TimeController> logger)
        {
            _rng = new Random();
            _logger = logger;
            CurrentWeather = new ExtraSunnyWeather(0);
            GameTime = new GameTime
            {
                Days = 0,
                Hours = 12,
                Minutes = 0
            };

            //_gameTimeTimer = new Timer
            //{
            //    Interval = TimerInterval
            //};
            //_gameTimeTimer.Elapsed += GameTimerOnElapsed;
            //_gameTimeTimer.AutoReset = true;
            //_gameTimeTimer.Start();

            _logger.LogInformation("Time manager initialized");
        }

        private void GameTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _elapsedMinutes++;
            UpdateTime();

            if (_elapsedMinutes < AppSettings.Current.ServerConfig.ChangeWeatherInterval)
                return;

            _elapsedMinutes = 0;
            ChangeWeather();
        }

        private void UpdateTime()
        {
            GameTime.Minutes += AppSettings.Current.ServerConfig.OneMinuteIrlToGameTime;
            _logger.LogDebug("Updated game time to {gameTime}", GameTime);
        }

        private void ChangeWeather()
        {
            var weatherChance = _rng.Next(0, 101);
            CurrentWeather = CurrentWeather.GetNextWeather(weatherChance);

            Alt.EmitAllClients("setWeatherOverTime", CurrentWeather.Weather, _weatherTransitionDuration);
            _logger.LogDebug("Updated weather to {weather}", CurrentWeather);
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
                if(value >= 24)
                {
                    _hours = value - 24;
                    Days++;
                }   
                else
                {
                    _hours = value;
                }
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                if (value >= 60)
                {
                    _minutes = value - 60;
                    Hours++;
                }
                else
                {
                    _minutes = value;
                }
            }
        }

        public override string ToString() => $"Day: {Days} Hour: {Hours} Minute: {Minutes}";
    }
}
