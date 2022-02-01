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
        public string Platform => AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo().OperatingSystem.ToString();
        public string Architecture => Environment.Is64BitOperatingSystem ? "x64" : "x86";
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(desktop.MainWindow)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}