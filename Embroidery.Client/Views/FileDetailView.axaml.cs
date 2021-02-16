using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Embroidery.Client.Models.View;

using System.Collections.ObjectModel;

namespace Embroidery.Client.Views
{
    public class FileDetailView : UserControl
    {
        public FileDetailView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
