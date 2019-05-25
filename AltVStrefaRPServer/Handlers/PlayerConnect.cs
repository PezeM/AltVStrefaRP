using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.Environment;
using AltVStrefaRPServer.Services.Characters;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerConnect
    {
        private readonly AppSettings _appSettings;
        private readonly ILogin _loginService;
        private readonly TimeManager _timeManager;

        public PlayerConnect(AppSettings appSettings, ILogin loginService, TimeManager timeManager)
        {
            _appSettings = appSettings;
            _loginService = loginService;
            _timeManager = timeManager;

            AltAsync.OnPlayerConnect += OnPlayerConnectAsync;
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
                var character = await _loginService.GetCharacterById(characterId).ConfigureAwait(false);
                if (character == null)
                {
                    Alt.Log($"Not found any character with id: {characterId}");
                    return;
                }

                // Trigger client-side event
                CharacterManager.Instance.IntializeCharacter(player, character);
                player.Emit("loadedCharacter");
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

                if (await _loginService.CheckIfAccountExistsAsync(login).ConfigureAwait(false) > 0)
                {
                    await player.EmitAsync("showLoginError", "Istnieje już konto z taką nazwą użytkownika.");
                    return;
                }

                //await _loginService.CreateNewAccountAndSaveAsync(login, password).ConfigureAwait(false);
                //await player.EmitAsync("successfullyRegistered");
                await Task.WhenAll(_loginService.CreateNewAccountAndSaveAsync(login, password), player.EmitAsync("successfullyRegistered"));
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

                var account = await _loginService.GetAccountAsync(login).ConfigureAwait(false);
                if (account == null)
                {
                    await player.EmitAsync("showLoginError", "Podano błędne dane.").ConfigureAwait(false);
                    return;
                }

                if (!_loginService.VerifyPassword(password, account.Password))
                {
                    await player.EmitAsync("showLoginError", "Podano błędne dane.").ConfigureAwait(false);
                    return;
                }

                player.SetData("accountId", account.AccountId);
                var characterList = await _loginService.GetCharacterList(account.AccountId).ConfigureAwait(false);
                await player.EmitAsync("loginSuccesfully", JsonConvert.SerializeObject(characterList));
                Alt.Log($"LoginAccount completed in {Time.GetTimestampMs() - startTime}ms.");
            }
            catch (Exception e)
            {
                Alt.Log($"[LoginAccount] Threw exception: {e}");
            }
        }

        private async Task OnPlayerConnectAsync(IPlayer player, string reason)
        {
            AltAsync.Log($"Player connected to the server: ID: {player.Id} Name: {player.Name} " +
                         $"Ping: {player.Ping}");
            try
            {
                await Task.WhenAll(player.SpawnAsync(new Position(_appSettings.ServerConfig.LoginPosition.X, _appSettings.ServerConfig.LoginPosition.Y,
                    _appSettings.ServerConfig.LoginPosition.Z)),
                    player.SetDateTimeAsync(_timeManager.GameTime.Days, 0, 0, _timeManager.GameTime.Hours, _timeManager.GameTime.Minutes, 0),
                    player.SetWeatherAsync(_timeManager.CurrentWeather));
                await player.EmitAsync("showAuthenticateWindow");
            }
            catch (Exception e)
            {
                Alt.Log($"[OnPlayerConnect] {e}");
            }
        }
    }
}
