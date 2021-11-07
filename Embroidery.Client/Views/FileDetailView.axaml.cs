using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Embroidery.Client.Models;
using Embroidery.Client.Models.View;
using Embroidery.Client.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Embroidery.Client.Views
{
    public class FileDetailView : UserControl
    {
        public FileDetailView()
        {
            InitializeComponent();
            this.DataContextChanged += FileDetailView_DataContextChanged;
        }

        private void FileDetailView_DataContextChanged(object sender, System.EventArgs e)
        {
            var addTagButton = this.FindControl<Button>("AddTag");
            var fileDetail = this.DataContext as Models.View.FileDetail;

            addTagButton.IsEnabled = (fileDetail != null && fileDetail.Tags.Any());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var imageControl = this.FindControl<Image>("FileImage");
            var simpleFile = ((Avalonia.Controls.DataGrid)sender).SelectedItem as Models.View.SimpleFile;

            if (simpleFile != null)
            {
                using (var db = new DataContext())
                {
                    var image = db.Files
                        .Where(x => x.Id == simpleFile.Id)
                        .Select(x => x.ImageThumbnail)
                        .FirstOrDefault();

                    if (image == null)
                        imageControl.Source = null;
                    else
                        imageControl.Source = Utilities.BitmapConverter.DbImageToBmpFile(simpleFile.Id, image);
                }
                System.Diagnostics.Debug.WriteLine($"Selected Data Grid changing image to {simpleFile.Id}");
            }            
        }

        public void DataGridDoubleTap(object sender, RoutedEventArgs e)
        {
            var simpleFile = ((Avalonia.Controls.DataGrid)sender).SelectedItem as Models.View.SimpleFile;

            if (simpleFile != null)
            {
                System.Diagnostics.Debug.WriteLine($"Double Tapped Data Grid {simpleFile.Path}");
                Process.Start("explorer.exe", $"/select, {System.IO.Path.Combine(simpleFile.Path,simpleFile.FullName)}");                
            }
        }

        public void AddTagClick(object sender, RoutedEventArgs e)
        {
            ((FileDetail)this.DataContext).AddTag();
        }

        public void RemoveTag(object sender, RoutedEventArgs e)
        {
            var item = (Avalonia.Controls.MenuItem)sender;
            var selectedTag = ((Avalonia.Controls.ListBox)item.Parent.Parent.Parent).SelectedItem as Tag;

            if(selectedTag != null)
                ((FileDetail)this.DataContext).RemoveTag(selectedTag);
            
            System.Diagnostics.Debug.WriteLine($"{nameof(RemoveTag)} '{selectedTag}'");            
        }
    }
}
