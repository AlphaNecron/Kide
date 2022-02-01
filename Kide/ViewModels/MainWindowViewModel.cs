using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.Rendering;
using Kide.Views;
using ReactiveUI;

namespace Kide.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
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
        public Interaction<SdkManagerViewModel, bool?> ShowDialog { get; }
        public ICommand OpenSdkManagerCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
        private string _code;
        private TextLocation _caretPos = new(1, 1);
        private Encoding _editorEncoding = Encoding.UTF8;
        private int _indentSize = 4;

        public string LinebreakChar { get; set; } = "CRLF";
        public MainWindowViewModel(Window w)
        {
            ShowDialog = new Interaction<SdkManagerViewModel,bool?>();
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await new OpenFileDialog
                {
                    AllowMultiple = false,
                    Title = "Open a file",
                    Filters =
                    {
                        new FileDialogFilter
                        {
                            Extensions = {"pas","inc","pp"},
                            Name = "Pascal program"
                        }
                    }
                }.ShowAsync(w);
                if (result!.Length > 0) Code = await File.ReadAllTextAsync(result.FirstOrDefault()!);
            });
            OpenSdkManagerCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialog.Handle(new SdkManagerViewModel());
            });
        }

    }
}