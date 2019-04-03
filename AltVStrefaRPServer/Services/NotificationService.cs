using AltV.Net.Elements.Entities;
using System.Threading.Tasks;
using AltV.Net.Async;

namespace AltVStrefaRPServer.Services
{
    public class NotificationService : INotificationService
    {
        public void ShowErrorNotfication(IPlayer player, string message, int time = 5000)
        {
            player.Emit("showNotification", 3, message, time);
        }

        public async Task ShowErrorNotificationAsync(IPlayer player, string message, int time = 5000)
        {
            await player.EmitAsync("showNotification", 3, message, time);
        }

        public void ShowInfoNotification(IPlayer player, string message, int time = 5000)
        {
            player.Emit("showNotification", 0, message, time);
        }

        public async Task ShowInfoNotificationAsync(IPlayer player, string message, int time = 5000)
        {
            await player.EmitAsync("showNotification", 0, message, time);
        }

        public void ShowWarningNotification(IPlayer player, string message, int time = 5000)
        {
            player.Emit("showNotification", 2, message, time);
        }

        public async Task ShowWarningNotificationAsync(IPlayer player, string message, int time = 5000)
        {
            await player.EmitAsync("showNotification", 2, message, time);
        }

        public void ShowSuccessNotification(IPlayer player, string message, int time = 5000)
        {
            player.Emit("showNotification", 1, message, time);
        }

        public async Task ShowSuccessNotificationAsync(IPlayer player, string message, int time = 5000)
        {
            await player.EmitAsync("showNotification", 1, message, time);
        }
    }
}
