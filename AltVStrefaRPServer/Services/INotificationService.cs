using AltV.Net.Elements.Entities;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services
{
    public interface INotificationService
    {
        void ShowErrorNotfication(IPlayer player, string message, int time = 5000);

        Task ShowErrorNotificationAsync(IPlayer player, string message, int time = 5000);

        void ShowInfoNotification(IPlayer player, string message, int time = 5000);

        Task ShowInfoNotificationAsync(IPlayer player, string message, int time = 5000);

        void ShowWarningNotification(IPlayer player, string message, int time = 5000);

        Task ShowWarningNotificationAsync(IPlayer player, string message, int time = 5000);

        void ShowSuccessNotification(IPlayer player, string message, int time = 5000);

        Task ShowSuccessNotificationAsync(IPlayer player, string message, int time = 5000);
    }
}
