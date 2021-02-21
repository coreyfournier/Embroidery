using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Embroidery.Client.Views
{
    public class SettingsDialog : Window
    {
        public SettingsDialog()
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
