using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Server;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Services.Characters;
using AltVStrefaRPServer.Services.Characters.Accounts;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerConnect
    {
        private readonly AppSettings _appSettings;
        private readonly ILogin _loginService;
        private readonly ICharacterDatabaseService _characterDatabaseService;
        private readonly IAccountDatabaseService _accountDatabaseService;
        private readonly TimeManager _timeManager;

        public PlayerConnect(AppSettings appSettings, ILogin loginService, ICharacterDatabaseService characterDatabaseService, 
            IAccountDatabaseService accountDatabaseService, TimeManager timeManager)
        {
            _appSettings = appSettings;
            _loginService = loginService;
            _timeManager = timeManager;
            _characterDatabaseService = characterDatabaseService;
            _accountDatabaseService = accountDatabaseService;

            Alt.OnPlayerConnect += OnPlayerConnect;
            AltAsync.On<IPlayer, string, string>("loginAccount", async (player, login, password) 
                => await LoginAccountAsync(player, login, password));
            AltAsync.On<IPlayer, string, string>("registerAccount", async (player, login, password) 
                => await RegisterAccountAsync(player, login, password));
            AltAsync.On<IPlayer, int>("tryToLoadCharacter", async (player, characterId) 
                => await TryToLoadCharacterAsync(player, characterId));
        }

        private async Task TryToLoadCharacterAsync(IPlayer player, int characterId)
        {
            try
            {
                var character = await _characterDatabaseService.GetCharacterById(characterId);
                if (character == null)
                {
                    Alt.Log($"Not found any character with id: {characterId}");
                    return;
                }

                await AltAsync.Do(() =>
                {
                    CharacterManager.Instance.IntializeCharacter(player, character);
                    // TODO: Add dropped items to stream
                    player.Emit("loadedCharacter");
                });
            }
            catch (Exception e)
            {
                Alt.Log($"[TryToLoadCharacterAsync] {e}");
            }
        }

        private async Task RegisterAccountAsync(IPlayer player, string login, string password)
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

                //await _loginService.CreateNewAccountAndSaveAsync(login, password).ConfigureAwait(false);
                //await player.EmitAsync("successfullyRegistered");
                await Task.WhenAll(_accountDatabaseService.CreateNewAccountAndSaveAsync(login, _loginService.GeneratePassword(password)), 
                    player.EmitAsync("successfullyRegistered"));
                AltAsync.Log($"Registered account in {Time.GetTimestampMs() - startTime}ms");
            }
            catch (Exception e)
            {
                AltAsync.Log($"[RegisterAccount] Threw exception: {e}");
            }
        }

        private async Task LoginAccountAsync(IPlayer player, string login, string password)
        {
            try
            {
                var startTime = Time.GetTimestampMs();
                Alt.Log($"Trying to login as {login}");

                if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
                {
                    Alt.Log($"Wrong data in try auth async");
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

                player.SetData("accountId", account.AccountId);
                await player.EmitAsync("loginSuccesfully", JsonConvert.SerializeObject(await _characterDatabaseService.GetCharacterList(account.AccountId)));
                Alt.Log($"LoginAccount completed in {Time.GetTimestampMs() - startTime}ms.");
            }
            catch (Exception e)
            {
                Alt.Log($"[LoginAccount] Threw exception: {e}");
            }
        }

        private void OnPlayerConnect(IPlayer player, string reason)
        {
            Alt.Log($"Player connected to the server: ID: {player.Id} Name: {player.Name} " +
                         $"Ping: {player.Ping}");

            player.Spawn(new Position(_appSettings.ServerConfig.LoginPosition.X, _appSettings.ServerConfig.LoginPosition.Y,
                _appSettings.ServerConfig.LoginPosition.Z));

            player.SetDateTime(_timeManager.GameTime.Days, 0, 0, _timeManager.GameTime.Hours, _timeManager.GameTime.Minutes, 0);
            player.SetWeather(_timeManager.CurrentWeather);
            player.Emit("showAuthenticateWindow");
        }
    }
}
