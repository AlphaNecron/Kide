using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit;
using Kide.Utils;

namespace Kide.Views
{
    public class ConsoleWindow : Window
    {
        public string FileToRun { get; set; }
        private TextEditor _console;
        private Process _runner;
        private Process _compiler;
        private ProcessStartInfo _runnerStartInfo;
        private ProcessStartInfo _compilerStartInfo;

        public ConsoleWindow()
        {
            InitializeComponent();
            Opened += async delegate
            {
                try
                {
                    _console!.Clear();
                    _runner = new Process();
                    _compiler = new Process();
                    var outputPath = Path.Combine(Path.GetTempPath(),
                        $"kide-{Path.GetRandomFileName().Replace(".", "")}");
                    var outputFile = Path.Combine(outputPath,
                        $"{Path.GetFileNameWithoutExtension(FileToRun)}{Helper.GetOsOutputExt()}");
                    _compilerStartInfo = new ProcessStartInfo
                    {
                        FileName = "fpc",
                        Arguments = $"{FileToRun} -o\"{outputFile}\"",
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    Log("Creating build directory");
                    Directory.CreateDirectory(outputPath);
                    _compiler.StartInfo = _compilerStartInfo;
                    _compiler.ErrorDataReceived += (_, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data)) Log($"[COMPILER/ERROR] {e.Data}");
                    };
                    _compiler.OutputDataReceived += (_, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data)) Log($"[COMPILER] {e.Data}");
                    };
                    _compiler.Start();
                    _compiler.BeginOutputReadLine();
                    _compiler.BeginErrorReadLine();
                    _compiler.EnableRaisingEvents = true;
                    _compiler.Exited += async (_, _) =>
                    {
                        if (_compiler.ExitCode != 0) return;
                        Clear();
                        _runnerStartInfo = new ProcessStartInfo
                        {
                            FileName = outputFile,
                            WorkingDirectory = outputPath,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                        _runner.StartInfo = _runnerStartInfo;
                        _runner.ErrorDataReceived += (_, e) =>
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data)) Log($"[ERROR] {e.Data}");
                        };
                        _runner.OutputDataReceived += (_, e) => Log($"{e.Data}");
                        _runner.Start();
                        _runner.BeginOutputReadLine();
                        _runner.BeginErrorReadLine();
                        await _runner.WaitForExitAsync();
                    };
                    await _compiler.WaitForExitAsync();
                }
                catch (Exception e)
                {
                    Log($"[EXCEPTION] {e}");
                }
            };
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private async void Clear() => await Dispatcher.UIThread.InvokeAsync(() => _console.Clear());

        private async void Log(string msg) => await Dispatcher.UIThread.InvokeAsync(() => _console.AppendText(msg + Environment.NewLine));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _console = this.FindControl<TextEditor>("Console");
            _console.TextArea.SelectionBrush = new SolidColorBrush
            {
                Color = Color.FromRgb(38, 78, 119),
                Opacity = 0.4
            };
            _console.TextArea.SelectionCornerRadius = 2;
            _console.TextArea.SelectionForeground = new SolidColorBrush
            {
                Color = Color.FromRgb(248, 250, 251)
            };
        }
    }
}