using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services.Characters;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerConnect
    {
        private AppSettings _appSettings;
        private ILogin _loginService;

        public PlayerConnect(AppSettings appSettings, ILogin loginService)
        {
            _appSettings = appSettings;
            _loginService = loginService;

            Alt.Log($"Player connect handler initialized.");
            AltAsync.OnPlayerConnect += OnPlayerConnectAsync;
            AltAsync.OnClient("loginAccount", LoginAccountAsync);
            AltAsync.OnClient("registerAccount", RegisterAccountAsync);
            AltAsync.OnClient("tryToLoadCharacter", TryToLoadCharacterAsync);
            async void function(IPlayer player, string login, string password)
            {
                AltAsync.Log($"Login account with arguments: {await player.GetNameAsync().ConfigureAwait(false)} login: {login} password: {password}.");
            }
            AltAsync.On<IPlayer, string, string>("loginAccount", function);
        }

        private async Task TryToLoadCharacterAsync(IPlayer player, object[] args)
        {
            try
            {
                var characterId = Convert.ToInt32(args[0]);
                if (characterId == 0) return;

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

        private async Task RegisterAccountAsync(IPlayer player, object[] args)
        {
            try
            {
                var startTime = Time.GetTimestampMs();
                var login = args[0].ToString();
                var password = args[1].ToString();
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    await player.EmitAsync("showLoginError", "Uzupełnij wszystkie pola");
                    return;
                }

                if (!_loginService.IsPasswordValid(password))
                {
                    await player.EmitAsync("showLoginError", "Hasło musi mieć 6-18 znaków, jedną cyfrę i jeden znak specjalny.");
                    return;
                }

                if (await _loginService.CheckIfAccountExistsAsync(login).ConfigureAwait(false))
                {
                    await player.EmitAsync("showLoginError", "Istnieje już konto z taką nazwą użytkownika.");
                    return;
                }

                await _loginService.CreateNewAccountAndSaveAsync(login, password).ConfigureAwait(false);
                await player.EmitAsync("successfullyRegistered");

                Alt.Log($"Registered account in {Time.GetTimestampMs() - startTime}ms");

            }
            catch (Exception e)
            {
                Alt.Log($"[RegisterAccount] Threw exception: {e}");
            }
        }

        private async Task LoginAccountAsync(IPlayer player, object[] args)
        {
            try
            {
                var startTime = Time.GetTimestampMs();
                string login = args[0].ToString();
                string password = args[1].ToString();
                Alt.Log($"Trying to login as {login}");

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(login))
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
                await player.SpawnAsync(new Position(_appSettings.ServerConfig.LoginPosition.X,
                    _appSettings.ServerConfig.LoginPosition.Y,
                    _appSettings.ServerConfig.LoginPosition.Z));

                await player.EmitAsync("showAuthenticateWindow");
            }
            catch (Exception e)
            {
                Alt.Log($"[OnPlayerConnect] {e}");
            }
        }
    }
}
