using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using Embroidery.Client.IO;
using Avalonia.Input;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.ObjectModel;
using Embroidery.Client.Models.View;

namespace Embroidery.Client.Views
{
    public class MainWindow : Window
    {
        ObservableCollection<Models.View.GroupedFile> observableCollection = new ObservableCollection<Models.View.GroupedFile>();
        string searchText = "";
        public MainWindow()
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

        /// <summary>
        /// Once the data context is set, start the crawler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);          
        }

        public void FileListItemSelected(object sender, GroupedFileEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Clicked {args.GroupedFile.CleanName}");

            //Find the file detail view by name
            var control = this.FindControl<FileDetailView>("FileDetail");

            using (var db = new DataContext())
            {
                var simpleFiles = db.SimpleFiles
                    .FromSqlInterpolated(@$"SELECT 
                        Files.Id,
                     [FullName],
                     Path
                    FROM 
                     [Files]
                    INNER JOIN Folders ON Folders.Id = FolderId
                    WHERE
                     CleanName = {args.GroupedFile.CleanName}");

                control.DataContext = new Models.View.FileDetail()
                {
                    GroupedFile = args.GroupedFile,
                    SimpleFiles = simpleFiles.ToArray(),
                    Tags = db.FileTagRelationships
                        .Where(x => x.FileId == args.GroupedFile.FirstFileId)
                        .Select(x => x.Tag)
                        .OrderBy(x => x.Name)
                        .ToArray()
                };
            }
        }

        public void SearchTextKeyUp(object sender, KeyEventArgs args)
        { 
            var textBox = sender as TextBox;
            var viewModel = this.DataContext as ViewModels.MainWindowViewModel;
            
            if (textBox != null && viewModel != null)
            {
                //Make sure there is a material change
                if (searchText != textBox.Text)
                {
                    viewModel.ExecuteSearch(textBox.Text);
                    searchText = textBox.Text;
                }
            }
        }        
    }
}
