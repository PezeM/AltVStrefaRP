using AltV.Net;
using System.Timers;
using AltVStrefaRPServer.Models.Enums;
using System;

namespace AltVStrefaRPServer.Modules.Environment
{
    public class TimeManager
    {
        private Timer _gameTimeTimer;
        private int _timerInterval = 60000;
        private int _elapsedMinutes = AppSettings.Current.ServerConfig.ChangeWeatherInterval == 0 
            ? 30 : AppSettings.Current.ServerConfig.ChangeWeatherInterval;
        private uint _currentWeather = (uint)Weathers.ExtraSunny;
        private GameTime _gameTime;

        public TimeManager()
        {
            _gameTime = new GameTime
            {
                Days = 0,
                Hours = 12,
                Minutes = 0
            };

            // Update time and weathers for all players on startup
            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetWeather(_currentWeather);
                player.SetDateTime(_gameTime.Days, 0, 0, _gameTime.Hours, _gameTime.Minutes, 0);
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
            UpdateTime();

            if (_elapsedMinutes >= AppSettings.Current.ServerConfig.ChangeWeatherInterval)
            {
                _elapsedMinutes = 0;
                ChangeWeather();
            }
        }

        private void UpdateTime()
        {
            _gameTime.Minutes += AppSettings.Current.ServerConfig.OneMinuteIrlToGameTime;
            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetDateTime(_gameTime.Days, 0, 0, _gameTime.Hours, _gameTime.Minutes, 0);
            }
        }

        private void ChangeWeather()
        {
            foreach (var player in Alt.GetAllPlayers())
            {
                player.SetWeather(_currentWeather);
            }
        }
    }

    public struct GameTime
    {
        public int Days { get; set; }

        public int Hours
        {
            get { return Hours; }
            set
            {
                Hours = value;
                if (Hours >= 24)
                {
                    Hours = 0;
                    Days++;
                }
            }
        }

        public int Minutes
        {
            get { return Minutes; }
            set
            {
                Minutes = value;
                if (Minutes >= 60)
                {
                    Minutes = 0;
                    Hours++;
                }
            }
        }
    }
}
