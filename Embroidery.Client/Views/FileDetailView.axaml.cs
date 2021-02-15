using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Embroidery.Client.Views
{
    public class FileDetailView : UserControl
    {
        public FileDetailView()
        {
            InitializeComponent();
            this.DataContextChanged += FileDetailView_DataContextChanged;
            
        }

        private void FileDetailView_DataContextChanged(object? sender, System.EventArgs e)
        {
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
