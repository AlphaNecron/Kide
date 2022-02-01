using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.TextMate.Grammars;
using Kide.ViewModels;
using ReactiveUI;
using DashStyle = Avalonia.Media.DashStyle;

namespace Kide.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private async Task DoShowDialogAsync(InteractionContext<SdkManagerViewModel, bool?> ctx)
        {
            var dialog = new SdkManagerDialog
            {
                DataContext = ctx.Input
            };
            var result = await dialog.ShowDialog<bool?>(this);
            ctx.SetOutput(result);
        }

        private void Init()
        {
            var ropts = new RegistryOptions(ThemeName.DarkPlus);
            var editor = this.FindControl<TextEditor>("MainEditor");
            editor.TextArea.Caret.PositionChanged += delegate
            {
                ViewModel!.CaretPos = editor.TextArea.Caret.Position.Location;
            };
            editor.TextArea.SelectionBrush = new SolidColorBrush
            {
                Color = Color.FromRgb(38, 78, 119),
                Opacity = 0.4
            };
            editor.TextArea.SelectionCornerRadius = 2;
            editor.TextArea.SelectionForeground = new SolidColorBrush
            {
                Color = Color.FromRgb(248, 250, 251)
            };
            editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
            var tm = editor.InstallTextMate(ropts);
            tm.SetGrammar(ropts.GetScopeByLanguageId(ropts.GetLanguageByExtension(".pas").Id));
            tm.SetTheme(ropts.LoadTheme(ThemeName.DarkPlus));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Init();
        }
    }
}