﻿using EasyExtract.Config;
using EasyExtract.Models;
using EasyExtract.Services;
using EasyExtract.Utilities;

namespace EasyExtract.Core;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly ConfigHelper _configHelper = new();

    private static async void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        await BetterLogger.LogAsync(e.Exception.Message, Importance.Error);
        // we cant show a Dialog here because the current doesn't have any active Window yet
        e.Handled = true;
    }

    private async void App_OnExit(object sender, ExitEventArgs e)
    {
        DiscordRpcManager.Instance.Dispose();
        base.OnExit(e);
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        DispatcherUnhandledException += Application_DispatcherUnhandledException;
        Exit += App_OnExit;

        if (_configHelper.Config.ContextMenuToggle)
        {
            // Run the program logic directly
            var program = new Program();
            _ = program.Run(e.Args);
        }
        else
        {
            _ = Program.DeleteContextMenu();
            InitializeComponent();
        }
    }
}