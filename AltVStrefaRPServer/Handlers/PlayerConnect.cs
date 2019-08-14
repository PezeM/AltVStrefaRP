using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Services.Characters;
using AltVStrefaRPServer.Services.Characters.Accounts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerConnect
    {
        private readonly AppSettings _appSettings;
        private readonly ILogin _loginService;
        private readonly ICharacterDatabaseService _characterDatabaseService;
        private readonly IAccountDatabaseService _accountDatabaseService;
        private readonly IAccountFactoryService _accountFactoryService;
        private readonly ILogger<PlayerConnect> _logger;
        private readonly TimeController _timeController;

        public PlayerConnect(AppSettings appSettings, ILogin loginService, ICharacterDatabaseService characterDatabaseService, 
            IAccountDatabaseService accountDatabaseService, IAccountFactoryService accountFactoryService, TimeController timeController, 
            ILogger<PlayerConnect> logger)
        {
            _appSettings = appSettings;
            _loginService = loginService;
            _timeController = timeController;
            _characterDatabaseService = characterDatabaseService;
            _accountDatabaseService = accountDatabaseService;
            _accountFactoryService = accountFactoryService;
            _logger = logger;

            Alt.OnPlayerConnect += OnPlayerConnect;
            AltAsync.On<IStrefaPlayer, string, string, Task>("LoginAccount", LoginAccountAsync);
            AltAsync.On<IStrefaPlayer, string, string, Task>("RegisterAccount", RegisterAccountAsync);
            AltAsync.On<IStrefaPlayer, int, Task>("TryToLoadCharacter", TryToLoadCharacterAsync);
        }

        private async Task TryToLoadCharacterAsync(IStrefaPlayer player, int characterId)
        {
            try
            {
                var character = await _characterDatabaseService.GetCharacterByIdAsync(characterId);
                if (character == null)
                {
                    // TODO: Emit event to player that cound't find character with given ID
                    _logger.LogWarning("Couldn't find character with ID({characterId)", characterId);
                    return;
                }

                await AltAsync.Do(() =>
                {
                    if (CharacterManager.Instance.IntializeCharacter(player, character))
                    {
                        player.Emit("loadedCharacter");
                    }
                    else
                    {
                        // Emit another event and enable button to choose character
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while trying to load charcter with ID({characterId})", characterId);
            }
        }

        private async Task RegisterAccountAsync(IStrefaPlayer player, string login, string password)
        {
            try
            {
                var startTime = Time.GetTimestampMs();
                if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
                {
                    await player.EmitAsync("showLoginError", "Uzupełnij wszystkie pola");
                    return;
                }

                if (!_loginService.IsPasswordValid(password))
                {
                    await player.EmitAsync("showLoginError", "Hasło musi mieć 6-18 znaków, jedną cyfrę i jeden znak specjalny.");
                    return;
                }

                if (await _accountDatabaseService.CheckIfAccountExistsAsync(login) > 0)
                {
                    await player.EmitAsync("showLoginError", "Istnieje już konto z taką nazwą użytkownika.");
                    return;
                }

                var account = _accountFactoryService.CreateNewAccount(login, _loginService.GeneratePassword(password));
                await _accountDatabaseService.AddNewAccountAsync(account).ConfigureAwait(false);
                await player.EmitAsync("successfullyRegistered");
                _logger.LogInformation("Registered new account {login} ID(accountId) in {elapsedTime}ms", login, account.AccountId, Time.GetElapsedTime(startTime));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in registering new account.");
            }
        }

        private async Task LoginAccountAsync(IStrefaPlayer player, string login, string password)
        {
            try
            {
                var startTime = Time.GetTimestampMs();
                _logger.LogDebug("Trying to login as {login}", login);

                if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
                {
                    await player.EmitAsync("showLoginError", "Uzupełnij wszystkie pola.");
                    return;
                }

                var account = await _accountDatabaseService.GetAccountAsync(login);
                if (account == null)
                {
                    await player.EmitAsync("showLoginError", "Podano błędne dane.");
                    return;
                }

                if (!_loginService.VerifyPassword(password, account.Password))
                {
                    await player.EmitAsync("showLoginError", "Podano błędne dane.");
                    return;
                }

                player.AccountId = account.AccountId;
                await player.EmitAsync("loggedInSuccesfully", JsonConvert.SerializeObject(await _characterDatabaseService.GetCharacterListAsync(account.AccountId)));
                _logger.LogInformation("Account {accountName} ID({accountId}) logged in successfully in completed in {elapsedTime}ms", account.Username, account.AccountId,
                    Time.GetElapsedTime(startTime));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while trying to login in account {login}", login);
            }
        }

        private void OnPlayerConnect(IPlayer player, string reason)
        {
            _logger.LogInformation("New player connected to the server. ID({playerId}) Name {playerName} Ip {playerIp}", 
                player.Id, player.Name, player.Ip);

            player.Spawn(new Position(_appSettings.ServerConfig.LoginPosition.X, _appSettings.ServerConfig.LoginPosition.Y,
                _appSettings.ServerConfig.LoginPosition.Z));

            player.SetDateTime(_timeController.GameTime.Days, 0, 0, _timeController.GameTime.Hours, _timeController.GameTime.Minutes, 0);
            player.SetWeather(_timeController.CurrentWeather);
            //player.Emit("showAuthenticateWindow");
        }
    }
}
