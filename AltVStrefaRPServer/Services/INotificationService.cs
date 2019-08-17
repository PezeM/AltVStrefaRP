using AltV.Net.Elements.Entities;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services
{
    public interface INotificationService
    {
        void ShowErrorNotification(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowErrorNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null);
        Task ShowErrorNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowInfoNotification(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowInfoNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null);
        Task ShowInfoNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowNoticeNotification(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowNoticeNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null);
        Task ShowNoticeNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowSuccessNotification(IPlayer player, string title, string message, int time = 5000, string icon = null);
        void ShowSuccessNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null);
        Task ShowSuccessNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null);
    }
}
