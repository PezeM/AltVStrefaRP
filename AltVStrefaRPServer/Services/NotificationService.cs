﻿using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services
{
    public class NotificationService : INotificationService
    {
        public void ShowErrorNotification(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.Emit("showNotification", 3, title, message, time, icon);
        }

        public async Task ShowErrorNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            await player.EmitAsync("showNotification", 3, title, message, time, icon);
        }

        public void ShowErrorNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.EmitLocked("showNotification", 3, title, message, time, icon);
        }

        public void ShowInfoNotification(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.Emit("showNotification", 0, title, message, time, icon);
        }

        public async Task ShowInfoNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            await player.EmitAsync("showNotification", 0, title, message, time, icon);
        }

        public void ShowInfoNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.EmitLocked("showNotification", 0, title, message, time, icon);
        }

        public void ShowNoticeNotification(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.Emit("showNotification", 2, title, message, time, icon);
        }

        public async Task ShowNoticeNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            await player.EmitAsync("showNotification", 2, title, message, time, icon);
        }

        public void ShowNoticeNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.EmitLocked("showNotification", 2, title, message, time, icon);
        }

        public void ShowSuccessNotification(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.Emit("showNotification", 1, title, message, time, icon);
        }

        public async Task ShowSuccessNotificationAsync(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            await player.EmitAsync("showNotification", 1, title, message, time, icon);
        }

        public void ShowSuccessNotificationLocked(IPlayer player, string title, string message, int time = 5000, string icon = null)
        {
            player.EmitLocked("showNotification", 1, title, message, time, icon);
        }
    }
}
