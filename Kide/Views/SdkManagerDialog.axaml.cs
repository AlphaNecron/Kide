using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kide.Views
{
    public partial class SdkManagerDialog : Window
    {
        public SdkManagerDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}