using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Kide.ViewModels;
using Kide.Views;

namespace Kide
{
    public class App : Application
    {
        public static OperatingSystemType? Platform => AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo()
            .OperatingSystem;

        public static string Architecture => Environment.Is64BitOperatingSystem ? "x64" : "x86";

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };

            base.OnFrameworkInitializationCompleted();
        }
    }
}