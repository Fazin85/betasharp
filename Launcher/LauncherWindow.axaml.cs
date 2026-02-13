using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Media;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace betareborn.Launcher
{
    internal sealed partial class LauncherWindow : Window
    {
        public static LaunchResult? Result { get; private set; }
        private readonly MicrosoftAuthService _authService = new();

        public LauncherWindow()
        {
            InitializeComponent();
        }

        private async void OnMicrosoftLogIn(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Opening browser for Microsoft login...";
                MicrosoftLoginButton.IsEnabled = false;

                var session = await _authService.AuthenticateAsync();

                if (session == null)
                {
                    StatusText.Text = "Login failed or was cancelled";
                    MicrosoftLoginButton.IsEnabled = true;
                    return;
                }

                StatusText.Text = $"Logged in as {session.Username}. Checking for Beta 1.7.3...";

                if (!File.Exists("b1.7.3.jar"))
                {
                    StatusText.Text = "Downloading Beta 1.7.3...";
                    var progressReporter = new Progress<double>(percent =>
                    {
                        StatusText.Text = $"Downloading Beta 1.7.3... {percent:F1}%";
                    });

                    var downloaded = await MinecraftDownloader.DownloadBeta173Async(progressReporter);
                    if (!downloaded)
                    {
                        StatusText.Text = "Failed to download Beta 1.7.3";
                        MicrosoftLoginButton.IsEnabled = true;
                        return;
                    }
                }

                StatusText.Text = "Ready to launch!";
                Result = new LaunchResult { Success = true, Session = session };
                Close();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                MicrosoftLoginButton.IsEnabled = true;
            }
        }

        private async void OnProvideJar(object sender, RoutedEventArgs e)
        {
            await PromptForJar();
        }



        private async Task PromptForJar()
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select b1.7.3.jar",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("JAR Files") { Patterns = new[] { "*.jar" } }
                }
            });

            if (files.Count > 0)
            {
                string selectedJar = files[0].Path.LocalPath;
                if (JarValidator.ValidateJar(selectedJar))
                {
                    File.Copy(selectedJar, "b1.7.3.jar", overwrite: true);
                    Result = new LaunchResult { Success = true, Session = null };
                    Close();
                }
                else
                {
                    StatusText.Text = "Invalid jar!";
                }
            }
        }
    }

    public class LaunchResult
    {
        public bool Success { get; set; }
        public Session? Session { get; set; }
    }

    public class Session
    {
        public string Username { get; set; } = "";
        public string AccessToken { get; set; } = "";
        public string Uuid { get; set; } = "";
    }
}