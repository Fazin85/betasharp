using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Microsoft.Win32;

namespace BetaSharp.Launcher;

public partial class LauncherWindow : Window
{
    public static LaunchResult? Result { get; private set; }
    private readonly MicrosoftAuthService _authService = new();
    private bool _isDarkMode;

    public LauncherWindow()
    {
        InitializeComponent();
        DetectSystemTheme();
        ApplyTheme();
    }

    private void DetectSystemTheme()
    {
        // Only detect system theme on Windows
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _isDarkMode = true; // Default to dark mode on non-Windows systems
            return;
        }

        try
        {
            var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null)
            {
                object? value = key.GetValue("AppsUseLightTheme");
                if (value is int intValue)
                {
                    _isDarkMode = intValue == 0; // 0 = dark mode, 1 = light mode
                }
                else
                {
                    _isDarkMode = true; // Default to dark if can't read
                }
            }
            else
            {
                _isDarkMode = true; // Default to dark if registry key not found
            }
        }
        catch
        {
            _isDarkMode = true; // Default to dark if any error
        }
    }

    private void OnToggleTheme(object sender, RoutedEventArgs e)
    {
        _isDarkMode = !_isDarkMode;
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        if (_isDarkMode)
        {
            // Dark mode
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            ThemeToggleButton.Content = "☀️";
                
            // Set text colors to light for dark mode
            var lightColor = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            StatusText.Foreground = lightColor;
            MicrosoftLoginButton.Foreground = lightColor;
            ProvideJar.Foreground = lightColor;
            ThemeToggleButton.Foreground = lightColor;
        }
        else
        {
            // Light mode
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            ThemeToggleButton.Content = "🌙";
                
            // Set text colors to dark for light mode
            var darkColor = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            StatusText.Foreground = darkColor;
            MicrosoftLoginButton.Foreground = darkColor;
            ProvideJar.Foreground = darkColor;
            ThemeToggleButton.Foreground = darkColor;
        }
    }

    private async void OnMicrosoftLogin(object sender, RoutedEventArgs e)
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
            FileTypeFilter =
            [
                new FilePickerFileType("JAR Files") { Patterns = ["*.jar"] },
            ]
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