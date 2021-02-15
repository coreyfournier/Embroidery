using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Embroidery.Client.Views
{
    public class FileView : UserControl
    {
        public FileView()
        {
            InitializeComponent();
        }



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
