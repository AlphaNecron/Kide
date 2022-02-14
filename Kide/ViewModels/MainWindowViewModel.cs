using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using AvaloniaEdit.Document;
using DynamicData.Binding;
using Kide.Views;
using ReactiveUI;

namespace Kide.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TextLocation _caretPos = new(1, 1);
        private string _code = "";
        private string _currentFile;
        private Encoding _editorEncoding = Encoding.UTF8;
        private int _indentSize = 4;
        private bool _isModified;
        private MainWindow _mainWindow;

        public MainWindowViewModel()
        {
            ShowConsoleWindow = new Interaction<string, int?>();
            OpenWindow = new Interaction<Type, object?>();
            ShowOpenFileDialog = new Interaction<Unit, string?>();
            ShowSaveAsDialog = new Interaction<Unit, string?>();
            SaveFile = new Interaction<Unit, bool>();
            SaveAsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileName = await ShowSaveAsDialog.Handle(Unit.Default);
                if (!string.IsNullOrWhiteSpace(fileName)) CurrentFile = fileName;
            });
            SaveFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (string.IsNullOrWhiteSpace(CurrentFile))
                {
                    var fileName = await ShowSaveAsDialog.Handle(Unit.Default);
                    if (!string.IsNullOrWhiteSpace(fileName)) CurrentFile = fileName;
                }
                else
                    await SaveFile.Handle(Unit.Default);
            });
            ShowConsoleCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                SaveFileCommand.Execute();
                await ShowConsoleWindow.Handle(CurrentFile);
            }, this.WhenAnyValue(
                x => x.CurrentFile,
                currentFile => 
                    !string.IsNullOrWhiteSpace(currentFile)));
            OpenWindowCommand = ReactiveCommand.CreateFromTask<Type>(async wt => await OpenWindow.Handle(wt));
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var fileName = await ShowOpenFileDialog.Handle(Unit.Default);
                if (fileName != null) CurrentFile = fileName;
            });
        }

        public Encoding EditorEncoding
        {
            get => _editorEncoding;
            set => this.RaiseAndSetIfChanged(ref _editorEncoding, value);
        }

        public int IndentSize
        {
            get => _indentSize;
            set => this.RaiseAndSetIfChanged(ref _indentSize, value);
        }

        public bool IsModified
        {
            get => _isModified;
            set => this.RaiseAndSetIfChanged(ref _isModified, value);
        }

        public TextLocation CaretPos
        {
            get => _caretPos;
            set => this.RaiseAndSetIfChanged(ref _caretPos, value);
        }

        public string Code
        {
            get => _code;
            set => this.RaiseAndSetIfChanged(ref _code, value);
        }
        public Interaction<string, int?> ShowConsoleWindow { get; }
        public Interaction<Type, object?> OpenWindow { get; }
        public Interaction<Unit, string?> ShowSaveAsDialog { get; }
        public Interaction<Unit, bool> SaveFile { get; }
        public Interaction<Unit, string?> ShowOpenFileDialog { get; }
        public ReactiveCommand<Type, Unit> OpenWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAsCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveFileCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
        public ReactiveCommand<Unit,Unit> ShowConsoleCommand { get; set; }

        public string CurrentFile
        {
            get => _currentFile;
            set => this.RaiseAndSetIfChanged(ref _currentFile, value);
        }

        public string LinebreakChar { get; set; } = "CRLF";
    }
}