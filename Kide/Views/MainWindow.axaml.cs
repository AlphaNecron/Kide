using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.TextMate.Grammars;
using Kide.Utils;
using Kide.ViewModels;
using ReactiveUI;

namespace Kide.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private TextEditor _editor;

        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                d(ViewModel!.ShowOpenFileDialog.RegisterHandler(ShowOpenFileDialogAsync));
                d(ViewModel!.ShowSaveAsDialog.RegisterHandler(ShowSaveAsDialogAsync));
                d(ViewModel!.SaveFile.RegisterHandler(SaveFileAsync));
                d(ViewModel!.OpenWindow.RegisterHandler(OpenWindowAsync));
                d(ViewModel!.ShowConsoleWindow.RegisterHandler(ShowConsoleWindowAsync));
            });
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private async Task OpenWindowAsync(InteractionContext<Type, object?> ctx)
        {
            var result = await ((Window) Activator.CreateInstance(ctx.Input)!).ShowDialog<object?>(this);
            ctx.SetOutput(result);
        }

        private async Task ShowConsoleWindowAsync(InteractionContext<string, int?> ctx)
        {
            var dialog = new ConsoleWindow
            {
                FileToRun = ctx.Input
            };
            var exitCode = await dialog.ShowDialog<int?>(this);
            ctx.SetOutput(exitCode);
        }

        private void InitEditor()
        {
            var ropts = new RegistryOptions(ThemeName.DarkPlus);
            _editor = this.FindControl<TextEditor>("MainEditor");
            _editor.TextArea.Caret.PositionChanged += delegate
            {
                ViewModel!.CaretPos = _editor.TextArea.Caret.Position.Location;
            };
            _editor.TextArea.SelectionBrush = new SolidColorBrush
            {
                Color = Color.FromRgb(38, 78, 119),
                Opacity = 0.4
            };
            _editor.TextArea.SelectionCornerRadius = 2;
            _editor.TextArea.SelectionForeground = new SolidColorBrush
            {
                Color = Color.FromRgb(248, 250, 251)
            };
            _editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
            var tm = _editor.InstallTextMate(ropts);
            tm.SetTheme(ropts.LoadTheme(ThemeName.DarkPlus));
            tm.SetGrammar(ropts.GetScopeByLanguageId(ropts.GetLanguageByExtension(".pas").Id));
        }

        private async Task ShowOpenFileDialogAsync(InteractionContext<Unit, string?> ctx)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = "Open a Pascal program",
                Filters =
                {
                    new FileDialogFilter
                    {
                        Extensions = {"pas", "inc", "pp"},
                        Name = "Pascal program"
                    }
                }
            };
            var fileNames = await dialog.ShowAsync(this);
            var fileName = fileNames?.FirstOrDefault();
            ctx.SetOutput(fileName);
            if (string.IsNullOrWhiteSpace(fileName)) return;
            await using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = FileReader.OpenStream(fs, ViewModel!.EditorEncoding);
            _editor.Document.UndoStack.ClearAll();
            _editor.Clear();
            _editor.AppendText(await reader.ReadToEndAsync());
            ViewModel!.EditorEncoding = reader.CurrentEncoding;
        }

        private async Task ShowSaveAsDialogAsync(InteractionContext<Unit, string?> ctx)
        {
            var currentFile = ViewModel!.CurrentFile;
            var dialog = new SaveFileDialog
            {
                Title = "Save",
                DefaultExtension = "pas",
                InitialFileName = Path.GetFileName(currentFile),
                Directory = Path.GetDirectoryName(currentFile)
            };
            var fileName = await dialog.ShowAsync(this);
            ctx.SetOutput(fileName);
            if (string.IsNullOrWhiteSpace(fileName)) return;
            await using var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            var encoding = ViewModel!.EditorEncoding;
            var document = _editor.Document;
            var writer = new StreamWriter(fs, encoding);
            document?.WriteTextTo(writer);
            await writer.FlushAsync();
        }

        private async Task SaveFileAsync(InteractionContext<Unit, bool> ctx)
        {
            try
            {
                var encoding = ViewModel!.EditorEncoding;
                var document = _editor.Document;
                var text = document.GetText(0, document.TextLength);
                await File.WriteAllTextAsync(ViewModel!.CurrentFile, text, encoding);
                ctx.SetOutput(true);
            }
            catch (Exception)
            {
                ctx.SetOutput(false);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            InitEditor();
        }
    }
}