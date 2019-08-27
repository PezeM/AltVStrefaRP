using AltV.Net;
using AltVStrefaRPServer.Models.Server;
using Serilog;
using System;
using System.Runtime.InteropServices;

namespace AltVStrefaRPServer
{
    internal static class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        // https://msdn.microsoft.com/fr-fr/library/windows/desktop/ms683242.aspx
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);

        private static void Main(string[] args)
        {
            SetConsoleCtrlHandler(ConsoleExitHandler, true);

            new Start().Start(args);
        }

        private static bool ConsoleExitHandler(CtrlType sig)
        {
            Log.Information("Inside ctrl c handler and about to close application.");
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    // Co some cleanup
                    Log.Information("Closing application. Closing and flushing logger");
                    Log.CloseAndFlush();
                    Environment.Exit(0);
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sig), sig, null);
            }

            return true;
        }
    }
}
