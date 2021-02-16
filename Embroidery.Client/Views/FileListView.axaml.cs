using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Markup.Xaml;
using System.Linq;

namespace Embroidery.Client.Views
{
    public class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void RowClicked(object sender, SelectionChangedEventArgs e)
        {
            //Find the file detail view by name
            var control = this.Parent.Parent.Parent.FindControl<FileDetailView>("FileDetail");

            if (e.AddedItems.Count > 0)
            {
                var groupedFile = e.AddedItems[0] as Models.View.GroupedFile;
                control.DataContext = groupedFile;

                if(groupedFile != null)
                    System.Diagnostics.Debug.WriteLine($"Clicked {groupedFile.CleanName}");
            }            
        }
    }
}
